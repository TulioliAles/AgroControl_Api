using AgroControl.Application.Inventory;
using AgroControl.Domain.Inventory;
using Microsoft.EntityFrameworkCore;

namespace AgroControl.Infrastructure.Persistence.Repositories;

internal sealed class StockLotRepository(AgroControlDbContext dbContext)
    : IStockLotRepository
{
    public Task<StockLot?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.StockLots
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<StockLot?> GetForUpdateByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.StockLots
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<StockLot> Items, int TotalCount)> ListAsync(
        int page,
        int pageSize,
        Guid? agriculturalInputId,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.StockLots.AsNoTracking();

        if (agriculturalInputId.HasValue)
        {
            query = query.Where(x => x.AgriculturalInputId == agriculturalInputId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.ExpirationDate)
            .ThenBy(x => x.LotNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<StockMovement>> ListMovementsAsync(
        Guid stockLotId,
        CancellationToken cancellationToken = default) =>
        await dbContext.StockMovements
            .AsNoTracking()
            .Where(x => x.StockLotId == stockLotId)
            .OrderByDescending(x => x.OccurredAt)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsAsync(
        Guid agriculturalInputId,
        string lotNumber,
        CancellationToken cancellationToken = default) =>
        dbContext.StockLots.AnyAsync(
            x => x.AgriculturalInputId == agriculturalInputId &&
                 x.LotNumber == lotNumber.Trim(),
            cancellationToken);

    public void Add(StockLot stockLot) => dbContext.StockLots.Add(stockLot);
}
