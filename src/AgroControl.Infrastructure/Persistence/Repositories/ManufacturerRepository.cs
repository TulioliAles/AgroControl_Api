using AgroControl.Application.Catalog.Repositories;
using AgroControl.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace AgroControl.Infrastructure.Persistence.Repositories;

internal sealed class ManufacturerRepository(AgroControlDbContext dbContext)
    : IManufacturerRepository
{
    public Task<Manufacturer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Manufacturers.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
}
