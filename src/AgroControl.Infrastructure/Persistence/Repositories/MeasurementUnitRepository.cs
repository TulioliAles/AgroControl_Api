using AgroControl.Application.Catalog.Repositories;
using AgroControl.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace AgroControl.Infrastructure.Persistence.Repositories;

internal sealed class MeasurementUnitRepository(AgroControlDbContext dbContext)
    : IMeasurementUnitRepository
{
    public Task<MeasurementUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.MeasurementUnits.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<MeasurementUnit?> GetForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.MeasurementUnits.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<MeasurementUnit> Items, int TotalCount)> ListAsync(
        int page, int pageSize, string? search, bool? isActive, CancellationToken cancellationToken = default)
    {
        var query = dbContext.MeasurementUnits.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.Name.Contains(term) || x.Symbol.Contains(term));
        }
        if (isActive.HasValue) query = query.Where(x => x.IsActive == isActive.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<bool> ExistsBySymbolAsync(string symbol, CancellationToken cancellationToken = default) =>
        dbContext.MeasurementUnits.AnyAsync(x => x.Symbol == symbol.Trim(), cancellationToken);

    public void Add(MeasurementUnit measurementUnit) => dbContext.MeasurementUnits.Add(measurementUnit);
}
