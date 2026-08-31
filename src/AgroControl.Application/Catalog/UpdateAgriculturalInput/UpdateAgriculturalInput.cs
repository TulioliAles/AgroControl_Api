using AgroControl.Application.Abstractions.Data;
using AgroControl.Application.Catalog.Repositories;
using AgroControl.Domain.Catalog;
using AgroControl.Domain.Common;

namespace AgroControl.Application.Catalog.UpdateAgriculturalInput;

public sealed record UpdateAgriculturalInputCommand(
    Guid Id,
    string Name,
    string? CommercialName,
    AgriculturalInputType Type,
    Guid CategoryId,
    Guid ManufacturerId,
    Guid MeasurementUnitId);

public sealed class UpdateAgriculturalInputHandler(
    IAgriculturalInputRepository agriculturalInputRepository,
    IInputCategoryRepository inputCategoryRepository,
    IManufacturerRepository manufacturerRepository,
    IMeasurementUnitRepository measurementUnitRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> HandleAsync(
        UpdateAgriculturalInputCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var agriculturalInput = await agriculturalInputRepository.GetForUpdateByIdAsync(
            command.Id,
            cancellationToken);

        if (agriculturalInput is null)
        {
            return Result.Failure(CatalogErrors.AgriculturalInputNotFound);
        }

        if (!string.Equals(agriculturalInput.Name, command.Name.Trim(), StringComparison.OrdinalIgnoreCase) &&
            await agriculturalInputRepository.ExistsByNameAsync(command.Name, cancellationToken))
        {
            return Result.Failure(CatalogErrors.AgriculturalInputNameAlreadyExists);
        }

        var category = await inputCategoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
        if (category is null || !category.IsActive)
        {
            return Result.Failure(CatalogErrors.InputCategoryNotFound);
        }

        var manufacturer = await manufacturerRepository.GetByIdAsync(command.ManufacturerId, cancellationToken);
        if (manufacturer is null || !manufacturer.IsActive)
        {
            return Result.Failure(CatalogErrors.ManufacturerNotFound);
        }

        var measurementUnit = await measurementUnitRepository.GetByIdAsync(
            command.MeasurementUnitId,
            cancellationToken);
        if (measurementUnit is null || !measurementUnit.IsActive)
        {
            return Result.Failure(CatalogErrors.MeasurementUnitNotFound);
        }

        agriculturalInput.Update(
            command.Name,
            command.CommercialName,
            command.Type,
            command.CategoryId,
            command.ManufacturerId,
            command.MeasurementUnitId);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed class ChangeAgriculturalInputStatusHandler(
    IAgriculturalInputRepository repository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> HandleAsync(
        Guid id,
        bool activate,
        CancellationToken cancellationToken = default)
    {
        var agriculturalInput = await repository.GetForUpdateByIdAsync(id, cancellationToken);
        if (agriculturalInput is null)
        {
            return Result.Failure(CatalogErrors.AgriculturalInputNotFound);
        }

        if (activate)
        {
            agriculturalInput.Activate();
        }
        else
        {
            agriculturalInput.Deactivate();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
