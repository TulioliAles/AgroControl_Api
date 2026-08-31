using AgroControl.Api.Contracts.Inventory;
using AgroControl.Api.Extensions;
using AgroControl.Api.Validation;
using AgroControl.Application.Common;
using AgroControl.Application.Inventory;
using AgroControl.Domain.Inventory;

namespace AgroControl.Api.Endpoints;

public static class StockLotEndpoints
{
    public static IEndpointRouteBuilder MapStockLotEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/stock-lots")
            .WithTags("Stock Lots")
            .RequireAuthorization();

        group.MapGet("/", ListAsync)
            .WithName("ListStockLots")
            .Produces<PagedResult<StockLotResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetStockLotById")
            .Produces<StockLotResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}/movements", ListMovementsAsync)
            .WithName("ListStockLotMovements")
            .Produces<IReadOnlyList<StockMovementResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateAsync)
            .Validate<CreateStockLotRequest>()
            .WithName("CreateStockLot")
            .Produces<CreatedStockLotResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/entries", RegisterEntryAsync)
            .Validate<RecordStockMovementRequest>()
            .WithName("RegisterStockEntry")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/exits", RegisterExitAsync)
            .Validate<RecordStockMovementRequest>()
            .WithName("RegisterStockExit")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static async Task<IResult> ListAsync(
        [AsParameters] ListStockLotsRequest request,
        StockLotQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.ListAsync(
            new ListStockLotsQuery(
                request.Page,
                request.PageSize,
                request.AgriculturalInputId,
                request.IsActive),
            cancellationToken);

        return Results.Ok(result);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        StockLotQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.GetAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToProblemResult();
    }

    private static async Task<IResult> ListMovementsAsync(
        Guid id,
        StockLotQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.ListMovementsAsync(id, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToProblemResult();
    }

    private static async Task<IResult> CreateAsync(
        CreateStockLotRequest request,
        CreateStockLotHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateStockLotCommand(
                request.AgriculturalInputId,
                request.LotNumber,
                request.ExpirationDate),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/stock-lots/{result.Value.Id}", result.Value)
            : result.Error.ToProblemResult();
    }

    private static Task<IResult> RegisterEntryAsync(
        Guid id,
        RecordStockMovementRequest request,
        RecordStockMovementHandler handler,
        CancellationToken cancellationToken) =>
        RegisterMovementAsync(
            id,
            StockMovementType.Entry,
            request,
            handler,
            cancellationToken);

    private static Task<IResult> RegisterExitAsync(
        Guid id,
        RecordStockMovementRequest request,
        RecordStockMovementHandler handler,
        CancellationToken cancellationToken) =>
        RegisterMovementAsync(
            id,
            StockMovementType.Exit,
            request,
            handler,
            cancellationToken);

    private static async Task<IResult> RegisterMovementAsync(
        Guid id,
        StockMovementType type,
        RecordStockMovementRequest request,
        RecordStockMovementHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new RecordStockMovementCommand(
                id,
                type,
                request.Quantity,
                request.OccurredAt ?? DateTimeOffset.UtcNow,
                request.Notes),
            cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToProblemResult();
    }
}
