using AgroControl.Domain.Catalog;

namespace AgroControl.Application.Catalog.Repositories;

public interface IManufacturerRepository
{
    Task<Manufacturer?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
