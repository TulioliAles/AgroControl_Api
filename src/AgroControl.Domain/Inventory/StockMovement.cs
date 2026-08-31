using AgroControl.Domain.Common;

namespace AgroControl.Domain.Inventory;

public sealed class StockMovement : Entity
{
    private StockMovement(
        Guid id,
        Guid stockLotId,
        StockMovementType type,
        decimal quantity,
        DateTimeOffset occurredAt,
        string? notes)
        : base(id)
    {
        StockLotId = stockLotId;
        Type = type;
        Quantity = quantity;
        OccurredAt = occurredAt;
        Notes = notes;
    }

    public Guid StockLotId { get; private set; }
    public StockMovementType Type { get; private set; }
    public decimal Quantity { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string? Notes { get; private set; }

    internal static StockMovement Create(
        Guid stockLotId,
        StockMovementType type,
        decimal quantity,
        DateTimeOffset occurredAt,
        string? notes)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                "The movement quantity must be greater than zero.");
        }

        var normalizedNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        if (normalizedNotes?.Length > 500)
        {
            throw new ArgumentOutOfRangeException(
                nameof(notes),
                normalizedNotes.Length,
                "Notes cannot exceed 500 characters.");
        }

        return new StockMovement(
            Guid.NewGuid(),
            Guard.AgainstEmpty(stockLotId, nameof(stockLotId)),
            type,
            quantity,
            occurredAt,
            normalizedNotes);
    }
}
