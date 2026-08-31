using AgroControl.Application.Abstractions.Data;
using AgroControl.Application.Catalog.Repositories;
using AgroControl.Domain.Catalog;
using AgroControl.Domain.Common;

namespace AgroControl.Application.Catalog.CreateAgriculturalInput;

public sealed class CreateAgriculturalInputHandler(
    IAgriculturalInputRepository agriculturalInputRepository,
    IInputCategoryRepository inputCategoryRepository,
    IManufacturerRepository manufacturerRepository,
    IMeasurementUnitRepository measurementUnitRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<CreateAgriculturalInputResponse>> HandleAsync(
        CreateAgriculturalInputCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (await agriculturalInputRepository.ExistsByNameAsync(command.Name, cancellationToken))
        {
            return Result<CreateAgriculturalInputResponse>.Failure(
                CatalogErrors.AgriculturalInputNameAlreadyExists);
        }

        var category = await inputCategoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
        if (category is null || !category.IsActive)
        {
            return Result<CreateAgriculturalInputResponse>.Failure(
                CatalogErrors.InputCategoryNotFound);
        }

        var manufacturer = await manufacturerRepository.GetByIdAsync(
            command.ManufacturerId,
            cancellationToken);

        if (manufacturer is null || !manufacturer.IsActive)
        {
            return Result<CreateAgriculturalInputResponse>.Failure(
                CatalogErrors.ManufacturerNotFound);
        }

        var measurementUnit = await measurementUnitRepository.GetByIdAsync(
            command.MeasurementUnitId,
            cancellationToken);

        if (measurementUnit is null || !measurementUnit.IsActive)
        {
            return Result<CreateAgriculturalInputResponse>.Failure(
                CatalogErrors.MeasurementUnitNotFound);
        }

        var agriculturalInput = AgriculturalInput.Create(
            command.Name,
            command.CommercialName,
            command.Type,
            command.CategoryId,
            command.ManufacturerId,
            command.MeasurementUnitId);

        agriculturalInputRepository.Add(agriculturalInput);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CreateAgriculturalInputResponse>.Success(
            new CreateAgriculturalInputResponse(agriculturalInput.Id));
    }
}
