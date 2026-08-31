using AgroControl.Domain.Catalog;

namespace AgroControl.Application.Catalog.Repositories;

public interface IAgriculturalInputRepository
{
    Task<AgriculturalInput?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<AgriculturalInput> Items, int TotalCount)> ListAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    void Add(AgriculturalInput agriculturalInput);
}
