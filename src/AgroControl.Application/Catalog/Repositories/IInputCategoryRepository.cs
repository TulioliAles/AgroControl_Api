using AgroControl.Domain.Catalog;

namespace AgroControl.Application.Catalog.Repositories;

public interface IInputCategoryRepository
{
    Task<InputCategory?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
