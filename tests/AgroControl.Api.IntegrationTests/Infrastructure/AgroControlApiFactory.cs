using System.Net.Http.Headers;
using System.Net.Http.Json;
using AgroControl.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace AgroControl.Api.IntegrationTests.Infrastructure;

public sealed class AgroControlApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string TestJwtKey = "integration-tests-only-secret-key-with-32-characters";

    private readonly MsSqlContainer _database = new MsSqlBuilder(
        "mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
        .Build();

    public HttpClient Client { get; private set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:AgroControlDatabase"] = _database.GetConnectionString(),
                ["Jwt:Issuer"] = "AgroControl.Api.IntegrationTests",
                ["Jwt:Audience"] = "AgroControl.Api.IntegrationTests.Client",
                ["Jwt:Key"] = TestJwtKey,
                ["Jwt:ExpirationMinutes"] = "60"
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _database.StartAsync();
        Client = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using (var scope = Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AgroControlDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        var suffix = Guid.NewGuid().ToString("N");
        var email = $"integration-{suffix}@agrocontrol.test";
        var password = "Integration#123";

        var registration = await Client.PostAsJsonAsync(
            "/api/auth/register",
            new { name = "Integration Tests", email, password });
        registration.EnsureSuccessStatusCode();

        var login = await Client.PostAsJsonAsync(
            "/api/auth/login",
            new { email, password });
        login.EnsureSuccessStatusCode();

        var token = await login.Content.ReadFromJsonAsync<AccessTokenPayload>();
        ArgumentNullException.ThrowIfNull(token);
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.AccessToken);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        Client.Dispose();
        await _database.DisposeAsync();
        Dispose();
    }

    public async Task<TResult> ExecuteDbContextAsync<TResult>(
        Func<AgroControlDbContext, Task<TResult>> action)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AgroControlDbContext>();
        return await action(dbContext);
    }

    private sealed record AccessTokenPayload(string AccessToken, DateTimeOffset ExpiresAt);
}
