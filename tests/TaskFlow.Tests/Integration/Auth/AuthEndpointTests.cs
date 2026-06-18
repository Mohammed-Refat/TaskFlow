using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskFlow.Core.DTOs.Auth;
using TaskFlow.Tests.Integration.Helpers;
using Xunit;

namespace TaskFlow.Tests.Integration.Auth;

public class AuthEndpointTests : IClassFixture<TaskFlowWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly TaskFlowWebAppFactory _factory;

    public AuthEndpointTests(TaskFlowWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_Returns200AndToken()
    {
        await _factory.ResetDatabaseAsync();

        var request = new
        {
            firstName = "Mohamed",
            lastName = "Refat",
            email = "mohamed@taskflow.com",
            password = "Test@1234"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        body!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns400()
    {
        await _factory.ResetDatabaseAsync();

        var request = new
        {
            firstName = "Mohamed",
            lastName = "Refat",
            email = "duplicate@taskflow.com",
            password = "Test@1234"
        };

        await _client.PostAsJsonAsync("/api/auth/register", request);
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_Returns401()
    {
        await _factory.ResetDatabaseAsync();

        await AuthHelper.RegisterAndLoginAsync(_client);

        var loginRequest = new
        {
            email = "test@taskflow.com",
            password = "WrongPassword"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200AndToken()
    {
        await _factory.ResetDatabaseAsync();

        await AuthHelper.RegisterAndLoginAsync(_client);

        var loginRequest = new
        {
            email = "test@taskflow.com",
            password = "Test@1234"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}