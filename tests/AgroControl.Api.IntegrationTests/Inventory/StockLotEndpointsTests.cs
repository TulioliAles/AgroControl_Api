using System.Net;
using System.Net.Http.Json;
using AgroControl.Api.IntegrationTests.Infrastructure;
using AgroControl.Domain.Catalog;

namespace AgroControl.Api.IntegrationTests.Inventory;

public sealed class StockLotEndpointsTests(AgroControlApiFactory factory)
    : IClassFixture<AgroControlApiFactory>
{
    [Fact]
    public async Task StockFlow_ShouldPersistBalanceAndMovements()
    {
        var suffix = Guid.NewGuid().ToString("N");
        var categoryId = await CreateAsync(
            "/api/input-categories",
            new { name = $"Categoria-{suffix}", description = (string?)null });
        var manufacturerId = await CreateAsync(
            "/api/manufacturers",
            new { name = $"Fabricante-{suffix}", registrationNumber = (string?)null });
        var measurementUnitId = await CreateAsync(
            "/api/measurement-units",
            new { name = $"Litro-{suffix}", symbol = $"L-{suffix[..8]}", conversionFactor = 1m });
        var inputId = await CreateAsync(
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
        var stockLotId = await CreateAsync(
            "/api/stock-lots",
            new
            {
                agriculturalInputId = inputId,
                lotNumber = $"LOT-{suffix[..10]}",
                expirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1))
            });

        var entry = await factory.Client.PostAsJsonAsync(
            $"/api/stock-lots/{stockLotId}/entries",
            new { quantity = 100m, occurredAt = DateTimeOffset.UtcNow, notes = "Initial entry" });
        var exit = await factory.Client.PostAsJsonAsync(
            $"/api/stock-lots/{stockLotId}/exits",
            new { quantity = 35m, occurredAt = DateTimeOffset.UtcNow, notes = "Field use" });
        var lotResponse = await factory.Client.GetAsync($"/api/stock-lots/{stockLotId}");
        var movementsResponse = await factory.Client.GetAsync(
            $"/api/stock-lots/{stockLotId}/movements");

        Assert.Equal(HttpStatusCode.NoContent, entry.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, exit.StatusCode);
        Assert.Equal(HttpStatusCode.OK, lotResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, movementsResponse.StatusCode);

        var lot = await lotResponse.Content.ReadFromJsonAsync<StockLotPayload>();
        var movements = await movementsResponse.Content.ReadFromJsonAsync<MovementPayload[]>();

        Assert.NotNull(lot);
        Assert.Equal(65m, lot.CurrentQuantity);
        Assert.NotNull(movements);
        Assert.Equal(2, movements.Length);
    }

    [Fact]
    public async Task ExitAboveBalance_ShouldReturnConflict()
    {
        var response = await factory.Client.PostAsJsonAsync(
            $"/api/stock-lots/{Guid.NewGuid()}/exits",
            new { quantity = 1m, occurredAt = DateTimeOffset.UtcNow, notes = (string?)null });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<Guid> CreateAsync(string uri, object request)
    {
        var response = await factory.Client.PostAsJsonAsync(uri, request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<CreatedPayload>();
        Assert.NotNull(payload);
        return payload.Id;
    }

    private sealed record CreatedPayload(Guid Id);
    private sealed record StockLotPayload(Guid Id, decimal CurrentQuantity);
    private sealed record MovementPayload(Guid Id, decimal Quantity);
}
