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
                ["ConnectionStrings:AgroControlDatabase"] = _database.GetConnectionString()
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

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AgroControlDbContext>();
        await dbContext.Database.MigrateAsync();
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
}
