using System.Net;
using System.Net.Http.Json;
using AgroControl.Api.IntegrationTests.Infrastructure;

namespace AgroControl.Api.IntegrationTests.Catalog;

public sealed class ReferenceDataMaintenanceTests(AgroControlApiFactory factory)
    : IClassFixture<AgroControlApiFactory>
{
    [Fact]
    public async Task CategoryLifecycle_ShouldPersistUpdateAndStatus()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var create = await factory.Client.PostAsJsonAsync(
            "/api/input-categories",
            new { name = $"Categoria-{suffix}", description = "Inicial" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<CreatedResponse>();
        Assert.NotNull(created);

        var update = await factory.Client.PutAsJsonAsync(
            $"/api/input-categories/{created.Id}",
            new { name = $"Categoria Atualizada-{suffix}", description = "Atualizada" });
        var deactivate = await factory.Client.PatchAsync(
            $"/api/input-categories/{created.Id}/deactivate",
            null);
        var get = await factory.Client.GetAsync($"/api/input-categories/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, update.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deactivate.StatusCode);
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        var payload = await get.Content.ReadFromJsonAsync<CategoryResponse>();
        Assert.NotNull(payload);
        Assert.Equal($"Categoria Atualizada-{suffix}", payload.Name);
        Assert.False(payload.IsActive);
    }

    private sealed record CreatedResponse(Guid Id);
    private sealed record CategoryResponse(Guid Id, string Name, string? Description, bool IsActive);
}
