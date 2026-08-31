using AgroControl.Application.Catalog.Repositories;
using AgroControl.Application.Common;
using AgroControl.Domain.Catalog;
using AgroControl.Domain.Common;

namespace AgroControl.Application.Catalog.GetAgriculturalInputs;

public sealed record AgriculturalInputResponse(
    Guid Id,
    string Name,
    string? CommercialName,
    AgriculturalInputType Type,
    Guid CategoryId,
    Guid ManufacturerId,
    Guid MeasurementUnitId,
    bool IsActive);

public sealed record ListAgriculturalInputsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool? IsActive = null);

public sealed class GetAgriculturalInputByIdHandler(
    IAgriculturalInputRepository repository)
{
    public async Task<Result<AgriculturalInputResponse>> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);

        return entity is null
            ? Result<AgriculturalInputResponse>.Failure(CatalogErrors.AgriculturalInputNotFound)
            : Result<AgriculturalInputResponse>.Success(Map(entity));
    }

    internal static AgriculturalInputResponse Map(AgriculturalInput entity) => new(
        entity.Id,
        entity.Name,
        entity.CommercialName,
        entity.Type,
        entity.CategoryId,
        entity.ManufacturerId,
        entity.MeasurementUnitId,
        entity.IsActive);
}

public sealed class ListAgriculturalInputsHandler(
    IAgriculturalInputRepository repository)
{
    public async Task<PagedResult<AgriculturalInputResponse>> HandleAsync(
        ListAgriculturalInputsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var result = await repository.ListAsync(
            page,
            pageSize,
            query.Search,
            query.IsActive,
            cancellationToken);

        return new PagedResult<AgriculturalInputResponse>(
            result.Items.Select(GetAgriculturalInputByIdHandler.Map).ToArray(),
            page,
            pageSize,
            result.TotalCount);
    }
}
