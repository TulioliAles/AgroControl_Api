using AgroControl.Application.Abstractions.Data;
using AgroControl.Application.Catalog.Repositories;
using AgroControl.Application.Common;
using AgroControl.Domain.Common;
using AgroControl.Domain.Inventory;

namespace AgroControl.Application.Inventory;

public sealed record CreateStockLotCommand(
    Guid AgriculturalInputId,
    string LotNumber,
    DateOnly? ExpirationDate);

public sealed record RecordStockMovementCommand(
    Guid StockLotId,
    StockMovementType Type,
    decimal Quantity,
    DateTimeOffset OccurredAt,
    string? Notes);

public sealed record ListStockLotsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? AgriculturalInputId = null,
    bool? IsActive = null);

public sealed record StockLotResponse(
    Guid Id,
    Guid AgriculturalInputId,
    string LotNumber,
    DateOnly? ExpirationDate,
    decimal CurrentQuantity,
    bool IsActive);

public sealed record StockMovementResponse(
    Guid Id,
    Guid StockLotId,
    StockMovementType Type,
    decimal Quantity,
    DateTimeOffset OccurredAt,
    string? Notes);

public sealed record CreatedStockLotResponse(Guid Id);

public sealed class CreateStockLotHandler(
    IStockLotRepository stockLotRepository,
    IAgriculturalInputRepository agriculturalInputRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<CreatedStockLotResponse>> HandleAsync(
        CreateStockLotCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var input = await agriculturalInputRepository.GetByIdAsync(
            command.AgriculturalInputId,
            cancellationToken);

        if (input is null || !input.IsActive)
        {
            return Result<CreatedStockLotResponse>.Failure(
                InventoryErrors.AgriculturalInputNotAvailable);
        }

        if (await stockLotRepository.ExistsAsync(
            command.AgriculturalInputId,
            command.LotNumber,
            cancellationToken))
        {
            return Result<CreatedStockLotResponse>.Failure(
                InventoryErrors.StockLotAlreadyExists);
        }

        var stockLot = StockLot.Create(
            command.AgriculturalInputId,
            command.LotNumber,
            command.ExpirationDate);

        stockLotRepository.Add(stockLot);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CreatedStockLotResponse>.Success(new(stockLot.Id));
    }
}

public sealed class RecordStockMovementHandler(
    IStockLotRepository repository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> HandleAsync(
        RecordStockMovementCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var stockLot = await repository.GetForUpdateByIdAsync(
            command.StockLotId,
            cancellationToken);

        if (stockLot is null)
        {
            return Result.Failure(InventoryErrors.StockLotNotFound);
        }

        if (!stockLot.IsActive)
        {
            return Result.Failure(InventoryErrors.InactiveStockLot);
        }

        if (command.Quantity <= 0)
        {
            return Result.Failure(InventoryErrors.InvalidQuantity);
        }

        if (command.Type == StockMovementType.Exit &&
            command.Quantity > stockLot.CurrentQuantity)
        {
            return Result.Failure(InventoryErrors.InsufficientStock);
        }

        if (command.Type == StockMovementType.Entry)
        {
            stockLot.Receive(command.Quantity, command.OccurredAt, command.Notes);
        }
        else
        {
            stockLot.Issue(command.Quantity, command.OccurredAt, command.Notes);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class StockLotQueryHandler(IStockLotRepository repository)
{
    public async Task<Result<StockLotResponse>> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var stockLot = await repository.GetByIdAsync(id, cancellationToken);
        return stockLot is null
            ? Result<StockLotResponse>.Failure(InventoryErrors.StockLotNotFound)
            : Result<StockLotResponse>.Success(Map(stockLot));
    }

    public async Task<PagedResult<StockLotResponse>> ListAsync(
        ListStockLotsQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var result = await repository.ListAsync(
            page,
            pageSize,
            query.AgriculturalInputId,
            query.IsActive,
            cancellationToken);

        return new(
            result.Items.Select(Map).ToArray(),
            page,
            pageSize,
            result.TotalCount);
    }

    public async Task<Result<IReadOnlyList<StockMovementResponse>>> ListMovementsAsync(
        Guid stockLotId,
        CancellationToken cancellationToken = default)
    {
        var stockLot = await repository.GetByIdAsync(stockLotId, cancellationToken);
        if (stockLot is null)
        {
            return Result<IReadOnlyList<StockMovementResponse>>.Failure(
                InventoryErrors.StockLotNotFound);
        }

        var movements = await repository.ListMovementsAsync(stockLotId, cancellationToken);
        return Result<IReadOnlyList<StockMovementResponse>>.Success(
            movements.Select(Map).ToArray());
    }

    private static StockLotResponse Map(StockLot entity) => new(
        entity.Id,
        entity.AgriculturalInputId,
        entity.LotNumber,
        entity.ExpirationDate,
        entity.CurrentQuantity,
        entity.IsActive);

    private static StockMovementResponse Map(StockMovement entity) => new(
        entity.Id,
        entity.StockLotId,
        entity.Type,
        entity.Quantity,
        entity.OccurredAt,
        entity.Notes);
}
