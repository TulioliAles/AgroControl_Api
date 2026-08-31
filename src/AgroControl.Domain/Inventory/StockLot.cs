using AgroControl.Domain.Common;

namespace AgroControl.Domain.Inventory;

public sealed class StockLot : AggregateRoot
{
    private readonly List<StockMovement> _movements = [];

    private StockLot(
        Guid id,
        Guid agriculturalInputId,
        string lotNumber,
        DateOnly? expirationDate)
        : base(id)
    {
        AgriculturalInputId = agriculturalInputId;
        LotNumber = lotNumber;
        ExpirationDate = expirationDate;
        CurrentQuantity = 0m;
        IsActive = true;
    }

    public Guid AgriculturalInputId { get; private set; }
    public string LotNumber { get; private set; }
    public DateOnly? ExpirationDate { get; private set; }
    public decimal CurrentQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<StockMovement> Movements => _movements.AsReadOnly();

    public static StockLot Create(
        Guid agriculturalInputId,
        string lotNumber,
        DateOnly? expirationDate = null)
    {
        return new StockLot(
            Guid.NewGuid(),
            Guard.AgainstEmpty(agriculturalInputId, nameof(agriculturalInputId)),
            Guard.AgainstNullOrWhiteSpace(lotNumber, nameof(lotNumber), 100),
            expirationDate);
    }

    public void Receive(
        decimal quantity,
        DateTimeOffset occurredAt,
        string? notes = null)
    {
        EnsureActive();
        var movement = StockMovement.Create(
            Id,
            StockMovementType.Entry,
            quantity,
            occurredAt,
            notes);

        CurrentQuantity += quantity;
        _movements.Add(movement);
    }

    public void Issue(
        decimal quantity,
        DateTimeOffset occurredAt,
        string? notes = null)
    {
        EnsureActive();

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                "The movement quantity must be greater than zero.");
        }

        if (quantity > CurrentQuantity)
        {
            throw new InvalidOperationException("Insufficient stock for this movement.");
        }

        var movement = StockMovement.Create(
            Id,
            StockMovementType.Exit,
            quantity,
            occurredAt,
            notes);

        CurrentQuantity -= quantity;
        _movements.Add(movement);
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    private void EnsureActive()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Stock movements cannot be recorded for an inactive lot.");
        }
    }
}
