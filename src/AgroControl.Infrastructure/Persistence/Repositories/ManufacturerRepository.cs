using AgroControl.Application.Catalog.Repositories;
using AgroControl.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace AgroControl.Infrastructure.Persistence.Repositories;

internal sealed class ManufacturerRepository(AgroControlDbContext dbContext)
    : IManufacturerRepository
{
    public Task<Manufacturer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Manufacturers.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Manufacturer?> GetForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Manufacturers.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Manufacturer> Items, int TotalCount)> ListAsync(
        int page, int pageSize, string? search, bool? isActive, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Manufacturers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.Name.Contains(term) || (x.RegistrationNumber != null && x.RegistrationNumber.Contains(term)));
        }
        if (isActive.HasValue) query = query.Where(x => x.IsActive == isActive.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
        dbContext.Manufacturers.AnyAsync(x => x.Name == name.Trim(), cancellationToken);

    public void Add(Manufacturer manufacturer) => dbContext.Manufacturers.Add(manufacturer);
}
