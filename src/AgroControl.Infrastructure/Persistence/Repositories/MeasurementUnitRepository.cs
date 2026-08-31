using AgroControl.Application.Catalog.Repositories;
using AgroControl.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace AgroControl.Infrastructure.Persistence.Repositories;

internal sealed class MeasurementUnitRepository(AgroControlDbContext dbContext)
    : IMeasurementUnitRepository
{
    public Task<MeasurementUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.MeasurementUnits.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsBySymbolAsync(string symbol, CancellationToken cancellationToken = default) =>
        dbContext.MeasurementUnits.AnyAsync(x => x.Symbol == symbol.Trim(), cancellationToken);

    public void Add(MeasurementUnit measurementUnit) => dbContext.MeasurementUnits.Add(measurementUnit);
}
