using AgroControl.Application.Catalog.Repositories;
using AgroControl.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace AgroControl.Infrastructure.Persistence.Repositories;

internal sealed class AgriculturalInputRepository(AgroControlDbContext dbContext)
    : IAgriculturalInputRepository
{
    public Task<AgriculturalInput?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.AgriculturalInputs.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
        dbContext.AgriculturalInputs.AnyAsync(x => x.Name == name.Trim(), cancellationToken);

    public void Add(AgriculturalInput agriculturalInput) =>
        dbContext.AgriculturalInputs.Add(agriculturalInput);
}
