using AgroControl.Api.Contracts.Catalog;
using AgroControl.Api.Contracts.Identity;
using AgroControl.Api.Contracts.Inventory;
using FluentValidation;

namespace AgroControl.Api.Validation;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(200);
    }
}

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(200);
    }
}

public sealed class CreateAgriculturalInputRequestValidator
    : AbstractValidator<CreateAgriculturalInputRequest>
{
    public CreateAgriculturalInputRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.CommercialName).MaximumLength(150);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.ManufacturerId).NotEmpty();
        RuleFor(x => x.MeasurementUnitId).NotEmpty();
    }
}

public sealed class UpdateAgriculturalInputRequestValidator
    : AbstractValidator<UpdateAgriculturalInputRequest>
{
    public UpdateAgriculturalInputRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.CommercialName).MaximumLength(150);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.ManufacturerId).NotEmpty();
        RuleFor(x => x.MeasurementUnitId).NotEmpty();
    }
}

public sealed class CreateInputCategoryRequestValidator
    : AbstractValidator<CreateInputCategoryRequest>
{
    public CreateInputCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public sealed class UpdateInputCategoryRequestValidator
    : AbstractValidator<UpdateInputCategoryRequest>
{
    public UpdateInputCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public sealed class CreateManufacturerRequestValidator
    : AbstractValidator<CreateManufacturerRequest>
{
    public CreateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.RegistrationNumber).MaximumLength(50);
    }
}

public sealed class UpdateManufacturerRequestValidator
    : AbstractValidator<UpdateManufacturerRequest>
{
    public UpdateManufacturerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.RegistrationNumber).MaximumLength(50);
    }
}

public sealed class CreateMeasurementUnitRequestValidator
    : AbstractValidator<CreateMeasurementUnitRequest>
{
    public CreateMeasurementUnitRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Symbol).NotEmpty().MaximumLength(20);
        RuleFor(x => x.ConversionFactor).GreaterThan(0);
    }
}

public sealed class UpdateMeasurementUnitRequestValidator
    : AbstractValidator<UpdateMeasurementUnitRequest>
{
    public UpdateMeasurementUnitRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Symbol).NotEmpty().MaximumLength(20);
        RuleFor(x => x.ConversionFactor).GreaterThan(0);
    }
}

public sealed class CreateStockLotRequestValidator : AbstractValidator<CreateStockLotRequest>
{
    public CreateStockLotRequestValidator()
    {
        RuleFor(x => x.AgriculturalInputId).NotEmpty();
        RuleFor(x => x.LotNumber).NotEmpty().MaximumLength(100);
    }
}

public sealed class RecordStockMovementRequestValidator
    : AbstractValidator<RecordStockMovementRequest>
{
    public RecordStockMovementRequestValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
