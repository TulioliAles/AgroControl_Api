using AgroControl.Api.Contracts.Catalog;
using AgroControl.Api.Extensions;
using AgroControl.Application.Catalog.CreateReferenceData;

namespace AgroControl.Api.Endpoints;

public static class CatalogReferenceDataEndpoints
{
    public static IEndpointRouteBuilder MapCatalogReferenceDataEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/input-categories", CreateInputCategoryAsync)
            .WithName("CreateInputCategory")
            .WithTags("Input Categories")
            .Produces<CreateCatalogReferenceResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

        endpoints.MapPost("/api/manufacturers", CreateManufacturerAsync)
            .WithName("CreateManufacturer")
            .WithTags("Manufacturers")
            .Produces<CreateCatalogReferenceResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

        endpoints.MapPost("/api/measurement-units", CreateMeasurementUnitAsync)
            .WithName("CreateMeasurementUnit")
            .WithTags("Measurement Units")
            .Produces<CreateCatalogReferenceResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static async Task<IResult> CreateInputCategoryAsync(
        CreateInputCategoryRequest request,
        CreateInputCategoryHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new CreateInputCategoryCommand(request.Name, request.Description),
                cancellationToken);

            return result.IsSuccess
                ? Results.Created($"/api/input-categories/{result.Value.Id}", result.Value)
                : result.Error.ToProblemResult();
        }
        catch (ArgumentException exception)
        {
            return exception.ToValidationProblem();
        }
    }

    private static async Task<IResult> CreateManufacturerAsync(
        CreateManufacturerRequest request,
        CreateManufacturerHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new CreateManufacturerCommand(request.Name, request.RegistrationNumber),
                cancellationToken);

            return result.IsSuccess
                ? Results.Created($"/api/manufacturers/{result.Value.Id}", result.Value)
                : result.Error.ToProblemResult();
        }
        catch (ArgumentException exception)
        {
            return exception.ToValidationProblem();
        }
    }

    private static async Task<IResult> CreateMeasurementUnitAsync(
        CreateMeasurementUnitRequest request,
        CreateMeasurementUnitHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new CreateMeasurementUnitCommand(
                    request.Name,
                    request.Symbol,
                    request.ConversionFactor),
                cancellationToken);

            return result.IsSuccess
                ? Results.Created($"/api/measurement-units/{result.Value.Id}", result.Value)
                : result.Error.ToProblemResult();
        }
        catch (ArgumentException exception)
        {
            return exception.ToValidationProblem();
        }
    }
}
