using AgroControl.Api.Contracts.Catalog;
using AgroControl.Api.Extensions;
using AgroControl.Application.Catalog.CreateAgriculturalInput;

namespace AgroControl.Api.Endpoints;

public static class AgriculturalInputEndpoints
{
    public static IEndpointRouteBuilder MapAgriculturalInputEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/agricultural-inputs")
            .WithTags("Agricultural Inputs");

        group.MapPost("/", CreateAsync)
            .WithName("CreateAgriculturalInput")
            .WithSummary("Creates a new agricultural input")
            .Produces<CreateAgriculturalInputResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateAgriculturalInputRequest request,
        CreateAgriculturalInputHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateAgriculturalInputCommand(
                request.Name,
                request.CommercialName,
                request.Type,
                request.CategoryId,
                request.ManufacturerId,
                request.MeasurementUnitId);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.IsSuccess
                ? Results.Created(
                    $"/api/agricultural-inputs/{result.Value.Id}",
                    result.Value)
                : result.Error.ToProblemResult();
        }
        catch (ArgumentException exception)
        {
            var field = exception.ParamName ?? "request";

            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [field] = [exception.Message]
            });
        }
    }
}
