using AgroControl.Domain.Catalog;

namespace AgroControl.Application.Catalog.Repositories;

public interface IManufacturerRepository
{
    Task<Manufacturer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Manufacturer?> GetForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        GetByIdAsync(id, cancellationToken);
    Task<(IReadOnlyList<Manufacturer> Items, int TotalCount)> ListAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("This repository does not support listing.");
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    void Add(Manufacturer manufacturer);
}
