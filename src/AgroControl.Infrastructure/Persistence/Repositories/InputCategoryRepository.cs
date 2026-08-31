using AgroControl.Application.Catalog.Repositories;
using AgroControl.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace AgroControl.Infrastructure.Persistence.Repositories;

internal sealed class InputCategoryRepository(AgroControlDbContext dbContext)
    : IInputCategoryRepository
{
    public Task<InputCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.InputCategories.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
}
