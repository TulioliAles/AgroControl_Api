using AgroControl.Application.Catalog.Repositories;
using AgroControl.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace AgroControl.Infrastructure.Persistence.Repositories;

internal sealed class InputCategoryRepository(AgroControlDbContext dbContext)
    : IInputCategoryRepository
{
    public Task<InputCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.InputCategories.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<InputCategory?> GetForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.InputCategories.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<InputCategory> Items, int TotalCount)> ListAsync(
        int page, int pageSize, string? search, bool? isActive, CancellationToken cancellationToken = default)
    {
        var query = dbContext.InputCategories.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.Name.Contains(term) || (x.Description != null && x.Description.Contains(term)));
        }
        if (isActive.HasValue) query = query.Where(x => x.IsActive == isActive.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
        dbContext.InputCategories.AnyAsync(x => x.Name == name.Trim(), cancellationToken);

    public void Add(InputCategory category) => dbContext.InputCategories.Add(category);
}
