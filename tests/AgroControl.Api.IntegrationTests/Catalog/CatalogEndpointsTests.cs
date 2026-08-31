using System.Net;
using System.Net.Http.Json;
using AgroControl.Api.IntegrationTests.Infrastructure;
using AgroControl.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace AgroControl.Api.IntegrationTests.Catalog;

public sealed class CatalogEndpointsTests(AgroControlApiFactory factory)
    : IClassFixture<AgroControlApiFactory>
{
    [Fact]
    public async Task CreateInputCategory_WithValidRequest_ShouldReturnCreatedAndPersist()
    {
        var name = $"Defensivos-{Guid.NewGuid():N}";

        var response = await factory.Client.PostAsJsonAsync(
            "/api/input-categories",
            new { name, description = "Controle de pragas" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CreatedResponse>();
        Assert.NotNull(payload);
        Assert.NotEqual(Guid.Empty, payload.Id);

        var persisted = await factory.ExecuteDbContextAsync(dbContext =>
            dbContext.InputCategories.AnyAsync(x => x.Id == payload.Id && x.Name == name));

        Assert.True(persisted);
    }

    [Fact]
    public async Task CreateInputCategory_WithDuplicatedName_ShouldReturnConflict()
    {
        var name = $"Fertilizantes-{Guid.NewGuid():N}";
        var request = new { name, description = "Nutrição vegetal" };

        var firstResponse = await factory.Client.PostAsJsonAsync(
            "/api/input-categories",
            request);
        var duplicateResponse = await factory.Client.PostAsJsonAsync(
            "/api/input-categories",
            request);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);

        var problem = await duplicateResponse.Content.ReadFromJsonAsync<ProblemResponse>();
        Assert.NotNull(problem);
        Assert.Equal("Catalog.InputCategory.NameAlreadyExists", problem.Code);
    }

    [Fact]
    public async Task CreateAgriculturalInput_WithValidReferences_ShouldReturnCreatedAndPersist()
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

        var inputName = $"Herbicida-{suffix}";
        var response = await factory.Client.PostAsJsonAsync(
            "/api/agricultural-inputs",
            new
            {
                name = inputName,
                commercialName = $"Campo-{suffix}",
                type = AgriculturalInputType.Pesticide,
                categoryId,
                manufacturerId,
                measurementUnitId
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CreatedResponse>();
        Assert.NotNull(payload);

        var persisted = await factory.ExecuteDbContextAsync(dbContext =>
            dbContext.AgriculturalInputs.AnyAsync(x =>
                x.Id == payload.Id &&
                x.Name == inputName &&
                x.CategoryId == categoryId &&
                x.ManufacturerId == manufacturerId &&
                x.MeasurementUnitId == measurementUnitId));

        Assert.True(persisted);
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
    private sealed record ProblemResponse(string Code);
}
