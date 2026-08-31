using AgroControl.Domain.Catalog;

namespace AgroControl.Api.Contracts.Catalog;

public sealed record CreateAgriculturalInputRequest(
    string Name,
    string? CommercialName,
    AgriculturalInputType Type,
    Guid CategoryId,
    Guid ManufacturerId,
    Guid MeasurementUnitId);
