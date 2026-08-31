using AgroControl.Domain.Catalog;

namespace AgroControl.Application.Catalog.Repositories;

public interface IInputCategoryRepository
{
    Task<InputCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InputCategory?> GetForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        GetByIdAsync(id, cancellationToken);
    Task<(IReadOnlyList<InputCategory> Items, int TotalCount)> ListAsync(
        int page,
        int pageSize,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("This repository does not support listing.");
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    void Add(InputCategory category);
}
