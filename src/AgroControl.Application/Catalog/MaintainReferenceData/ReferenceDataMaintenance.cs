using AgroControl.Application.Abstractions.Data;
using AgroControl.Application.Catalog.Repositories;
using AgroControl.Application.Common;
using AgroControl.Domain.Catalog;
using AgroControl.Domain.Common;

namespace AgroControl.Application.Catalog.MaintainReferenceData;

public sealed record InputCategoryResponse(Guid Id, string Name, string? Description, bool IsActive);
public sealed record ManufacturerResponse(Guid Id, string Name, string? RegistrationNumber, bool IsActive);
public sealed record MeasurementUnitResponse(Guid Id, string Name, string Symbol, decimal ConversionFactor, bool IsActive);
public sealed record ReferenceDataQuery(int Page = 1, int PageSize = 20, string? Search = null, bool? IsActive = null);

public sealed record UpdateInputCategoryCommand(Guid Id, string Name, string? Description);
public sealed record UpdateManufacturerCommand(Guid Id, string Name, string? RegistrationNumber);
public sealed record UpdateMeasurementUnitCommand(Guid Id, string Name, string Symbol, decimal ConversionFactor);

public sealed class InputCategoryMaintenanceHandler(IInputCategoryRepository repository, IUnitOfWork unitOfWork)
{
    public async Task<Result<InputCategoryResponse>> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        return entity is null
            ? Result<InputCategoryResponse>.Failure(CatalogErrors.InputCategoryNotFound)
            : Result<InputCategoryResponse>.Success(Map(entity));
    }

    public async Task<PagedResult<InputCategoryResponse>> ListAsync(ReferenceDataQuery query, CancellationToken ct = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var result = await repository.ListAsync(page, pageSize, query.Search, query.IsActive, ct);
        return new(result.Items.Select(Map).ToArray(), page, pageSize, result.TotalCount);
    }

    public async Task<Result> UpdateAsync(UpdateInputCategoryCommand command, CancellationToken ct = default)
    {
        var entity = await repository.GetForUpdateByIdAsync(command.Id, ct);
        if (entity is null) return Result.Failure(CatalogErrors.InputCategoryNotFound);
        if (!string.Equals(entity.Name, command.Name.Trim(), StringComparison.OrdinalIgnoreCase) && await repository.ExistsByNameAsync(command.Name, ct))
            return Result.Failure(CatalogErrors.InputCategoryNameAlreadyExists);
        entity.Update(command.Name, command.Description);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    public Task<Result> ActivateAsync(Guid id, CancellationToken ct = default) => ChangeStatusAsync(id, true, ct);
    public Task<Result> DeactivateAsync(Guid id, CancellationToken ct = default) => ChangeStatusAsync(id, false, ct);

    private async Task<Result> ChangeStatusAsync(Guid id, bool active, CancellationToken ct)
    {
        var entity = await repository.GetForUpdateByIdAsync(id, ct);
        if (entity is null) return Result.Failure(CatalogErrors.InputCategoryNotFound);
        if (active) entity.Activate(); else entity.Deactivate();
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static InputCategoryResponse Map(InputCategory x) => new(x.Id, x.Name, x.Description, x.IsActive);
}

public sealed class ManufacturerMaintenanceHandler(IManufacturerRepository repository, IUnitOfWork unitOfWork)
{
    public async Task<Result<ManufacturerResponse>> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        return entity is null ? Result<ManufacturerResponse>.Failure(CatalogErrors.ManufacturerNotFound) : Result<ManufacturerResponse>.Success(Map(entity));
    }

    public async Task<PagedResult<ManufacturerResponse>> ListAsync(ReferenceDataQuery query, CancellationToken ct = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var result = await repository.ListAsync(page, pageSize, query.Search, query.IsActive, ct);
        return new(result.Items.Select(Map).ToArray(), page, pageSize, result.TotalCount);
    }

    public async Task<Result> UpdateAsync(UpdateManufacturerCommand command, CancellationToken ct = default)
    {
        var entity = await repository.GetForUpdateByIdAsync(command.Id, ct);
        if (entity is null) return Result.Failure(CatalogErrors.ManufacturerNotFound);
        if (!string.Equals(entity.Name, command.Name.Trim(), StringComparison.OrdinalIgnoreCase) && await repository.ExistsByNameAsync(command.Name, ct))
            return Result.Failure(CatalogErrors.ManufacturerNameAlreadyExists);
        entity.Update(command.Name, command.RegistrationNumber);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    public Task<Result> ActivateAsync(Guid id, CancellationToken ct = default) => ChangeStatusAsync(id, true, ct);
    public Task<Result> DeactivateAsync(Guid id, CancellationToken ct = default) => ChangeStatusAsync(id, false, ct);

    private async Task<Result> ChangeStatusAsync(Guid id, bool active, CancellationToken ct)
    {
        var entity = await repository.GetForUpdateByIdAsync(id, ct);
        if (entity is null) return Result.Failure(CatalogErrors.ManufacturerNotFound);
        if (active) entity.Activate(); else entity.Deactivate();
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static ManufacturerResponse Map(Manufacturer x) => new(x.Id, x.Name, x.RegistrationNumber, x.IsActive);
}

public sealed class MeasurementUnitMaintenanceHandler(IMeasurementUnitRepository repository, IUnitOfWork unitOfWork)
{
    public async Task<Result<MeasurementUnitResponse>> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        return entity is null ? Result<MeasurementUnitResponse>.Failure(CatalogErrors.MeasurementUnitNotFound) : Result<MeasurementUnitResponse>.Success(Map(entity));
    }

    public async Task<PagedResult<MeasurementUnitResponse>> ListAsync(ReferenceDataQuery query, CancellationToken ct = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var result = await repository.ListAsync(page, pageSize, query.Search, query.IsActive, ct);
        return new(result.Items.Select(Map).ToArray(), page, pageSize, result.TotalCount);
    }

    public async Task<Result> UpdateAsync(UpdateMeasurementUnitCommand command, CancellationToken ct = default)
    {
        var entity = await repository.GetForUpdateByIdAsync(command.Id, ct);
        if (entity is null) return Result.Failure(CatalogErrors.MeasurementUnitNotFound);
        if (!string.Equals(entity.Symbol, command.Symbol.Trim(), StringComparison.OrdinalIgnoreCase) && await repository.ExistsBySymbolAsync(command.Symbol, ct))
            return Result.Failure(CatalogErrors.MeasurementUnitSymbolAlreadyExists);
        entity.Update(command.Name, command.Symbol, command.ConversionFactor);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    public Task<Result> ActivateAsync(Guid id, CancellationToken ct = default) => ChangeStatusAsync(id, true, ct);
    public Task<Result> DeactivateAsync(Guid id, CancellationToken ct = default) => ChangeStatusAsync(id, false, ct);

    private async Task<Result> ChangeStatusAsync(Guid id, bool active, CancellationToken ct)
    {
        var entity = await repository.GetForUpdateByIdAsync(id, ct);
        if (entity is null) return Result.Failure(CatalogErrors.MeasurementUnitNotFound);
        if (active) entity.Activate(); else entity.Deactivate();
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static MeasurementUnitResponse Map(MeasurementUnit x) => new(x.Id, x.Name, x.Symbol, x.ConversionFactor, x.IsActive);
}
