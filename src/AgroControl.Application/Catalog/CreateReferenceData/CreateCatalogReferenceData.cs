using AgroControl.Application.Abstractions.Data;
using AgroControl.Application.Catalog.Repositories;
using AgroControl.Domain.Catalog;
using AgroControl.Domain.Common;

namespace AgroControl.Application.Catalog.CreateReferenceData;

public sealed record CreateInputCategoryCommand(string Name, string? Description);
public sealed record CreateManufacturerCommand(string Name, string? RegistrationNumber);
public sealed record CreateMeasurementUnitCommand(string Name, string Symbol, decimal ConversionFactor);
public sealed record CreateCatalogReferenceResponse(Guid Id);

public sealed class CreateInputCategoryHandler(
    IInputCategoryRepository repository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<CreateCatalogReferenceResponse>> HandleAsync(
        CreateInputCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (await repository.ExistsByNameAsync(command.Name, cancellationToken))
        {
            return Result<CreateCatalogReferenceResponse>.Failure(
                CatalogErrors.InputCategoryNameAlreadyExists);
        }

        var category = InputCategory.Create(command.Name, command.Description);
        repository.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CreateCatalogReferenceResponse>.Success(
            new CreateCatalogReferenceResponse(category.Id));
    }
}

public sealed class CreateManufacturerHandler(
    IManufacturerRepository repository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<CreateCatalogReferenceResponse>> HandleAsync(
        CreateManufacturerCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (await repository.ExistsByNameAsync(command.Name, cancellationToken))
        {
            return Result<CreateCatalogReferenceResponse>.Failure(
                CatalogErrors.ManufacturerNameAlreadyExists);
        }

        var manufacturer = Manufacturer.Create(command.Name, command.RegistrationNumber);
        repository.Add(manufacturer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CreateCatalogReferenceResponse>.Success(
            new CreateCatalogReferenceResponse(manufacturer.Id));
    }
}

public sealed class CreateMeasurementUnitHandler(
    IMeasurementUnitRepository repository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<CreateCatalogReferenceResponse>> HandleAsync(
        CreateMeasurementUnitCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (await repository.ExistsBySymbolAsync(command.Symbol, cancellationToken))
        {
            return Result<CreateCatalogReferenceResponse>.Failure(
                CatalogErrors.MeasurementUnitSymbolAlreadyExists);
        }

        var unit = MeasurementUnit.Create(
            command.Name,
            command.Symbol,
            command.ConversionFactor);

        repository.Add(unit);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CreateCatalogReferenceResponse>.Success(
            new CreateCatalogReferenceResponse(unit.Id));
    }
}
