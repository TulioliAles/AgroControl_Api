using AgroControl.Domain.Catalog;

namespace AgroControl.Application.Catalog.Repositories;

public interface IMeasurementUnitRepository
{
    Task<MeasurementUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MeasurementUnit?> GetForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        GetByIdAsync(id, cancellationToken);
    Task<(IReadOnlyList<MeasurementUnit> Items, int TotalCount)> ListAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("This repository does not support listing.");
    Task<bool> ExistsBySymbolAsync(string symbol, CancellationToken cancellationToken = default);
    void Add(MeasurementUnit measurementUnit);
}
