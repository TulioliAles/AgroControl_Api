using System.Net;
using System.Net.Http.Json;
using AgroControl.Api.IntegrationTests.Infrastructure;
using AgroControl.Domain.Catalog;

namespace AgroControl.Api.IntegrationTests.Catalog;

public sealed class AgriculturalInputMaintenanceTests(AgroControlApiFactory factory)
    : IClassFixture<AgroControlApiFactory>
{
    [Fact]
    public async Task UpdateAndDeactivate_ShouldPersistChanges()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var categoryId = await CreateReferenceAsync(
            "/api/input-categories",
            new { name = $"Categoria-{suffix}", description = (string?)null });
        var manufacturerId = await CreateReferenceAsync(
            "/api/manufacturers",
            new { name = $"Fabricante-{suffix}", registrationNumber = (string?)null });
        var measurementUnitId = await CreateReferenceAsync(
            "/api/measurement-units",
            new { name = $"Litro-{suffix}", symbol = $"L-{suffix[..8]}", conversionFactor = 1m });
        var inputId = await CreateReferenceAsync(
            "/api/agricultural-inputs",
            new
            {
                name = $"Insumo-{suffix}",
                commercialName = (string?)null,
                type = AgriculturalInputType.Pesticide,
                categoryId,
                manufacturerId,
                measurementUnitId
            });

        var updateResponse = await factory.Client.PutAsJsonAsync(
            $"/api/agricultural-inputs/{inputId}",
            new
            {
                name = $"Insumo Atualizado-{suffix}",
                commercialName = $"Comercial-{suffix}",
                type = AgriculturalInputType.Fertilizer,
                categoryId,
                manufacturerId,
                measurementUnitId
            });
        var deactivateResponse = await factory.Client.PatchAsync(
            $"/api/agricultural-inputs/{inputId}/deactivate",
            null);
        var getResponse = await factory.Client.GetAsync($"/api/agricultural-inputs/{inputId}");

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var payload = await getResponse.Content.ReadFromJsonAsync<AgriculturalInputPayload>();
        Assert.NotNull(payload);
        Assert.Equal($"Insumo Atualizado-{suffix}", payload.Name);
        Assert.False(payload.IsActive);
    }

    private async Task<Guid> CreateReferenceAsync(string uri, object request)
    {
        var response = await factory.Client.PostAsJsonAsync(uri, request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<CreatedResponse>();
        Assert.NotNull(payload);
        return payload.Id;
    }

    private sealed record CreatedResponse(Guid Id);
    private sealed record AgriculturalInputPayload(Guid Id, string Name, bool IsActive);
}
