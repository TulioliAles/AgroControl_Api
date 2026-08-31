using AgroControl.Domain.Common;

namespace AgroControl.Application.Catalog;

public static class CatalogErrors
{
    public static readonly Error AgriculturalInputNameAlreadyExists = Error.Conflict(
        "Catalog.AgriculturalInput.NameAlreadyExists",
        "An agricultural input with the same name already exists.");

    public static readonly Error AgriculturalInputNotFound = Error.NotFound(
        "Catalog.AgriculturalInput.NotFound",
        "The agricultural input was not found.");

    public static readonly Error InputCategoryNameAlreadyExists = Error.Conflict(
        "Catalog.InputCategory.NameAlreadyExists",
        "An input category with the same name already exists.");

    public static readonly Error ManufacturerNameAlreadyExists = Error.Conflict(
        "Catalog.Manufacturer.NameAlreadyExists",
        "A manufacturer with the same name already exists.");

    public static readonly Error MeasurementUnitSymbolAlreadyExists = Error.Conflict(
        "Catalog.MeasurementUnit.SymbolAlreadyExists",
        "A measurement unit with the same symbol already exists.");

    public static readonly Error InputCategoryNotFound = Error.NotFound(
        "Catalog.InputCategory.NotFound",
        "The informed input category was not found or is inactive.");

    public static readonly Error ManufacturerNotFound = Error.NotFound(
        "Catalog.Manufacturer.NotFound",
        "The informed manufacturer was not found or is inactive.");

    public static readonly Error MeasurementUnitNotFound = Error.NotFound(
        "Catalog.MeasurementUnit.NotFound",
        "The informed measurement unit was not found or is inactive.");
}
