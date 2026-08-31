using AgroControl.Domain.Common;

namespace AgroControl.Application.Catalog;

public static class CatalogErrors
{
    public static readonly Error AgriculturalInputNameAlreadyExists = Error.Conflict(
        "Catalog.AgriculturalInput.NameAlreadyExists",
        "An agricultural input with the same name already exists.");

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
