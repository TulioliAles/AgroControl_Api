namespace AgroControl.Api.Contracts.Catalog;

public sealed record ReferenceDataListRequest(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool? IsActive = null);

public sealed record UpdateInputCategoryRequest(string Name, string? Description);
public sealed record UpdateManufacturerRequest(string Name, string? RegistrationNumber);
public sealed record UpdateMeasurementUnitRequest(string Name, string Symbol, decimal ConversionFactor);
