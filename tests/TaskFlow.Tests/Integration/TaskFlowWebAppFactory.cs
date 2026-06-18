using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using TaskFlow.Infrastructure.Data;
using Testcontainers.PostgreSql;
using Xunit;

namespace TaskFlow.Tests.Integration;

public class TaskFlowWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:16")
    .WithDatabase("taskflow_test")
    .WithUsername("postgres")
    .WithPassword("postgres")
    .Build();


    private Respawner _respawner = default!;
    private NpgsqlConnection _connection = default!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();

        _connection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"]
        });
    }


    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_connection);
    }


    async Task IAsyncLifetime.DisposeAsync()
    {
        await _connection.CloseAsync();
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));
        });
    }





}