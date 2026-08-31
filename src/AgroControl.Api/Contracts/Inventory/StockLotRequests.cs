namespace AgroControl.Api.Contracts.Inventory;

public sealed record CreateStockLotRequest(
    Guid AgriculturalInputId,
    string LotNumber,
    DateOnly? ExpirationDate);

public sealed record RecordStockMovementRequest(
    decimal Quantity,
    DateTimeOffset? OccurredAt,
    string? Notes);

public sealed record ListStockLotsRequest(
    int Page = 1,
    int PageSize = 20,
    Guid? AgriculturalInputId = null,
    bool? IsActive = null);
