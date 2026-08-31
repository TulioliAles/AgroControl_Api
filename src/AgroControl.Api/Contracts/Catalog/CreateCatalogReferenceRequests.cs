namespace AgroControl.Api.Contracts.Catalog;

public sealed record CreateInputCategoryRequest(string Name, string? Description);

public sealed record CreateManufacturerRequest(string Name, string? RegistrationNumber);

public sealed record CreateMeasurementUnitRequest(
    string Name,
    string Symbol,
    decimal ConversionFactor = 1m);
