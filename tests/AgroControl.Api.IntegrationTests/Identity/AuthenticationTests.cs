using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AgroControl.Api.IntegrationTests.Infrastructure;

namespace AgroControl.Api.IntegrationTests.Identity;

public sealed class AuthenticationTests(AgroControlApiFactory factory)
    : IClassFixture<AgroControlApiFactory>
{
    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ShouldReturnUnauthorized()
    {
        using var anonymousClient = factory.CreateClient();

        var response = await anonymousClient.GetAsync("/api/agricultural-inputs");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RegisterAndLogin_WithValidCredentials_ShouldReturnToken()
    {
        using var anonymousClient = factory.CreateClient();
        var suffix = Guid.NewGuid().ToString("N");
        var email = $"user-{suffix}@agrocontrol.test";
        const string password = "StrongPassword#123";

        var registerResponse = await anonymousClient.PostAsJsonAsync(
            "/api/auth/register",
            new { name = "Authentication Test", email, password });
        var loginResponse = await anonymousClient.PostAsJsonAsync(
            "/api/auth/login",
            new { email, password });

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var token = await loginResponse.Content.ReadFromJsonAsync<AccessTokenPayload>();
        Assert.NotNull(token);
        Assert.False(string.IsNullOrWhiteSpace(token.AccessToken));
        Assert.True(token.ExpiresAt > DateTimeOffset.UtcNow);

        anonymousClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.AccessToken);

        var protectedResponse = await anonymousClient.GetAsync("/api/agricultural-inputs");
        Assert.Equal(HttpStatusCode.OK, protectedResponse.StatusCode);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnBadRequest()
    {
        using var anonymousClient = factory.CreateClient();
        var suffix = Guid.NewGuid().ToString("N");
        var email = $"invalid-{suffix}@agrocontrol.test";

        await anonymousClient.PostAsJsonAsync(
            "/api/auth/register",
            new { name = "Invalid Login", email, password = "ValidPassword#123" });

        var response = await anonymousClient.PostAsJsonAsync(
            "/api/auth/login",
            new { email, password = "WrongPassword#123" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private sealed record AccessTokenPayload(string AccessToken, DateTimeOffset ExpiresAt);
}
