using AgroControl.Domain.Common;

namespace AgroControl.Domain.Catalog;

public sealed class MeasurementUnit : Entity
{
    private MeasurementUnit(Guid id, string name, string symbol, decimal conversionFactor)
        : base(id)
    {
        Name = name;
        Symbol = symbol;
        ConversionFactor = conversionFactor;
        IsActive = true;
    }

    public string Name { get; private set; }
    public string Symbol { get; private set; }
    public decimal ConversionFactor { get; private set; }
    public bool IsActive { get; private set; }

    public static MeasurementUnit Create(string name, string symbol, decimal conversionFactor = 1m)
    {
        var normalizedName = Guard.AgainstNullOrWhiteSpace(name, nameof(name), 100);
        var normalizedSymbol = Guard.AgainstNullOrWhiteSpace(symbol, nameof(symbol), 20);

        if (conversionFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(conversionFactor),
                conversionFactor,
                "The conversion factor must be greater than zero.");
        }

        return new MeasurementUnit(Guid.NewGuid(), normalizedName, normalizedSymbol, conversionFactor);
    }

    public void Update(string name, string symbol, decimal conversionFactor)
    {
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name), 100);
        Symbol = Guard.AgainstNullOrWhiteSpace(symbol, nameof(symbol), 20);

        if (conversionFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(conversionFactor),
                conversionFactor,
                "The conversion factor must be greater than zero.");
        }

        ConversionFactor = conversionFactor;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
