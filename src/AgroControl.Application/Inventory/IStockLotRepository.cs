using AgroControl.Domain.Inventory;

namespace AgroControl.Application.Inventory;

public interface IStockLotRepository
{
    Task<StockLot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StockLot?> GetForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<StockLot> Items, int TotalCount)> ListAsync(
        int page,
        int pageSize,
        Guid? agriculturalInputId,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockMovement>> ListMovementsAsync(
        Guid stockLotId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid agriculturalInputId,
        string lotNumber,
        CancellationToken cancellationToken = default);

    void Add(StockLot stockLot);
}
