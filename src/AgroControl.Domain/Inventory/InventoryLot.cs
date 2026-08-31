using AgroControl.Domain.Common;

namespace AgroControl.Domain.Inventory;

public sealed class InventoryLot : Entity
{
    private InventoryLot(
        Guid id,
        Guid agriculturalInputId,
        string lotNumber,
        decimal initialQuantity,
        DateOnly? manufacturingDate,
        DateOnly? expirationDate)
        : base(id)
    {
        if (agriculturalInputId == Guid.Empty)
        {
            throw new ArgumentException("O insumo é obrigatório.", nameof(agriculturalInputId));
        }

        if (string.IsNullOrWhiteSpace(lotNumber))
        {
            throw new ArgumentException("O número do lote é obrigatório.", nameof(lotNumber));
        }

        if (initialQuantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialQuantity), "A quantidade inicial deve ser maior que zero.");
        }

        if (manufacturingDate.HasValue && expirationDate.HasValue && expirationDate < manufacturingDate)
        {
            throw new ArgumentException("A validade não pode ser anterior à fabricação.", nameof(expirationDate));
        }

        AgriculturalInputId = agriculturalInputId;
        LotNumber = lotNumber.Trim();
        CurrentQuantity = initialQuantity;
        ManufacturingDate = manufacturingDate;
        ExpirationDate = expirationDate;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid AgriculturalInputId { get; private init; }
    public string LotNumber { get; private init; }
    public decimal CurrentQuantity { get; private set; }
    public DateOnly? ManufacturingDate { get; private init; }
    public DateOnly? ExpirationDate { get; private init; }
    public DateTime CreatedAtUtc { get; private init; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public bool IsExpired(DateOnly referenceDate) =>
        ExpirationDate.HasValue && ExpirationDate.Value < referenceDate;

    public static InventoryLot Create(
        Guid agriculturalInputId,
        string lotNumber,
        decimal initialQuantity,
        DateOnly? manufacturingDate = null,
        DateOnly? expirationDate = null)
    {
        return new InventoryLot(
            Guid.NewGuid(),
            agriculturalInputId,
            lotNumber,
            initialQuantity,
            manufacturingDate,
            expirationDate);
    }

    public void AddStock(decimal quantity)
    {
        EnsurePositive(quantity);
        CurrentQuantity += quantity;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RemoveStock(decimal quantity)
    {
        EnsurePositive(quantity);

        if (quantity > CurrentQuantity)
        {
            throw new InvalidOperationException("A quantidade solicitada é maior que o saldo disponível no lote.");
        }

        CurrentQuantity -= quantity;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static void EnsurePositive(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "A quantidade deve ser maior que zero.");
        }
    }
}
