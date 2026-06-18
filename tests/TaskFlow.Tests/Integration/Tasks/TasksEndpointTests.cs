using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskFlow.Core.DTOs.Tasks;
using TaskFlow.Core.Enums;
using TaskFlow.Tests.Integration.Helpers;
using Xunit;

namespace TaskFlow.Tests.Integration.Tasks;

public class TasksEndpointTests : IClassFixture<TaskFlowWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly TaskFlowWebAppFactory _factory;

    public TasksEndpointTests(TaskFlowWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private void AuthorizeClient(string token)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetAllTasks_WithoutToken_Returns401()
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.GetAsync("/api/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTask_WithValidData_Returns201()
    {
        await _factory.ResetDatabaseAsync();

        var token = await AuthHelper.RegisterAndLoginAsync(_client);
        AuthorizeClient(token);

        var request = new CreateTaskRequest
        {
            Title = "Test Task",
            Description = "Test Description",
            Priority = TaskItemPriority.High
        };

        var response = await _client.PostAsJsonAsync("/api/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetTaskById_WithValidId_Returns200()
    {
        await _factory.ResetDatabaseAsync();

        var token = await AuthHelper.RegisterAndLoginAsync(_client, "getbyid@taskflow.com");
        AuthorizeClient(token);

        var createRequest = new CreateTaskRequest { Title = "Test Task", Priority = TaskItemPriority.Medium };
        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();

        var response = await _client.GetAsync($"/api/tasks/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var task = await response.Content.ReadFromJsonAsync<TaskResponse>();
        task!.Title.Should().Be("Test Task");
    }

    [Fact]
    public async Task GetTaskById_WithOtherUsersTask_Returns404()
    {
        await _factory.ResetDatabaseAsync();

        // User A creates a task
        var tokenA = await AuthHelper.RegisterAndLoginAsync(_client, "userA@taskflow.com");
        AuthorizeClient(tokenA);
        var createRequest = new CreateTaskRequest { Title = "User A Task", Priority = TaskItemPriority.Medium };
        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();

        // User B tries to access User A's task
        var tokenB = await AuthHelper.RegisterAndLoginAsync(_client, "userB@taskflow.com");
        AuthorizeClient(tokenB);
        var response = await _client.GetAsync($"/api/tasks/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTask_WithValidId_Returns204()
    {
        await _factory.ResetDatabaseAsync();

        var token = await AuthHelper.RegisterAndLoginAsync(_client, "deletetask@taskflow.com");
        AuthorizeClient(token);

        var createRequest = new CreateTaskRequest { Title = "Task to delete", Priority = TaskItemPriority.Low };
        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();

        var response = await _client.DeleteAsync($"/api/tasks/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}