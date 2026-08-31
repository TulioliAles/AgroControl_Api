using AgroControl.Api.Contracts.Catalog;
using AgroControl.Api.Extensions;
using AgroControl.Application.Catalog.CreateAgriculturalInput;
using AgroControl.Application.Catalog.GetAgriculturalInputs;
using AgroControl.Application.Catalog.UpdateAgriculturalInput;
using AgroControl.Application.Common;

namespace AgroControl.Api.Endpoints;

public static class AgriculturalInputEndpoints
{
    public static IEndpointRouteBuilder MapAgriculturalInputEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/agricultural-inputs")
            .WithTags("Agricultural Inputs");

        group.MapGet("/", ListAsync)
            .WithName("ListAgriculturalInputs")
            .WithSummary("Lists agricultural inputs with pagination and filters")
            .Produces<PagedResult<AgriculturalInputResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetAgriculturalInputById")
            .WithSummary("Gets an agricultural input by identifier")
            .Produces<AgriculturalInputResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateAsync)
            .WithName("CreateAgriculturalInput")
            .WithSummary("Creates a new agricultural input")
            .Produces<CreateAgriculturalInputResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateAgriculturalInput")
            .WithSummary("Updates an agricultural input")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPatch("/{id:guid}/activate", ActivateAsync)
            .WithName("ActivateAgriculturalInput")
            .WithSummary("Activates an agricultural input")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPatch("/{id:guid}/deactivate", DeactivateAsync)
            .WithName("DeactivateAgriculturalInput")
            .WithSummary("Deactivates an agricultural input")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }

    private static async Task<IResult> ListAsync(
        [AsParameters] ListAgriculturalInputsRequest request,
        ListAgriculturalInputsHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new ListAgriculturalInputsQuery(
                request.Page,
                request.PageSize,
                request.Search,
                request.IsActive),
            cancellationToken);

        return Results.Ok(result);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        GetAgriculturalInputByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToProblemResult();
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
                ? Results.Created($"/api/agricultural-inputs/{result.Value.Id}", result.Value)
                : result.Error.ToProblemResult();
        }
        catch (ArgumentException exception)
        {
            return exception.ToValidationProblem();
        }
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateAgriculturalInputRequest request,
        UpdateAgriculturalInputHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.HandleAsync(
                new UpdateAgriculturalInputCommand(
                    id,
                    request.Name,
                    request.CommercialName,
                    request.Type,
                    request.CategoryId,
                    request.ManufacturerId,
                    request.MeasurementUnitId),
                cancellationToken);

            return result.IsSuccess
                ? Results.NoContent()
                : result.Error.ToProblemResult();
        }
        catch (ArgumentException exception)
        {
            return exception.ToValidationProblem();
        }
    }

    private static Task<IResult> ActivateAsync(
        Guid id,
        ChangeAgriculturalInputStatusHandler handler,
        CancellationToken cancellationToken) =>
        ChangeStatusAsync(id, true, handler, cancellationToken);

    private static Task<IResult> DeactivateAsync(
        Guid id,
        ChangeAgriculturalInputStatusHandler handler,
        CancellationToken cancellationToken) =>
        ChangeStatusAsync(id, false, handler, cancellationToken);

    private static async Task<IResult> ChangeStatusAsync(
        Guid id,
        bool activate,
        ChangeAgriculturalInputStatusHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(id, activate, cancellationToken);
        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToProblemResult();
    }
}
