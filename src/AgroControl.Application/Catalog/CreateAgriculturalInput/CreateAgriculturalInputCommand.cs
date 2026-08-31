using AgroControl.Domain.Catalog;

namespace AgroControl.Application.Catalog.CreateAgriculturalInput;

public sealed record CreateAgriculturalInputCommand(
    string Name,
    string? CommercialName,
    AgriculturalInputType Type,
    Guid CategoryId,
    Guid ManufacturerId,
    Guid MeasurementUnitId);
