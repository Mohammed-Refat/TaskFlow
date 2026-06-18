using System.Net.Http.Json;
using TaskFlow.Core.DTOs.Auth;

namespace TaskFlow.Tests.Integration.Helpers;

public static class AuthHelper
{
    public static async Task<string> RegisterAndLoginAsync(
        HttpClient client,
        string email = "test@taskflow.com",
        string password = "Test@1234")
    {
        var registerRequest = new
        {
            firstName = "Test",
            lastName = "User",
            email = email,
            password = password
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", registerRequest);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return result!.AccessToken;
    }
}