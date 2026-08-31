using AgroControl.Domain.Catalog;

namespace AgroControl.Application.Catalog.Repositories;

public interface IMeasurementUnitRepository
{
    Task<MeasurementUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySymbolAsync(string symbol, CancellationToken cancellationToken = default);
    void Add(MeasurementUnit measurementUnit);
}
