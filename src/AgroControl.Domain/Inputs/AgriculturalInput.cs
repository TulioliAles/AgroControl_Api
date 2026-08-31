using AgroControl.Domain.Common;

namespace AgroControl.Domain.Inputs;

public sealed class AgriculturalInput : Entity
{
    private AgriculturalInput(
        Guid id,
        string name,
        InputCategory category,
        MeasurementUnit measurementUnit,
        decimal minimumStock,
        string? manufacturer,
        string? externalRegistrationNumber)
        : base(id)
    {
        Name = NormalizeRequiredText(name, nameof(name), 200);
        Category = category;
        MeasurementUnit = measurementUnit;
        MinimumStock = ValidateNonNegative(minimumStock, nameof(minimumStock));
        Manufacturer = NormalizeOptionalText(manufacturer, 200);
        ExternalRegistrationNumber = NormalizeOptionalText(externalRegistrationNumber, 100);
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public string Name { get; private set; }
    public InputCategory Category { get; private set; }
    public MeasurementUnit MeasurementUnit { get; private set; }
    public decimal MinimumStock { get; private set; }
    public string? Manufacturer { get; private set; }
    public string? ExternalRegistrationNumber { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private init; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public static AgriculturalInput Create(
        string name,
        InputCategory category,
        MeasurementUnit measurementUnit,
        decimal minimumStock = 0,
        string? manufacturer = null,
        string? externalRegistrationNumber = null)
    {
        return new AgriculturalInput(
            Guid.NewGuid(),
            name,
            category,
            measurementUnit,
            minimumStock,
            manufacturer,
            externalRegistrationNumber);
    }

    public void Update(
        string name,
        InputCategory category,
        MeasurementUnit measurementUnit,
        decimal minimumStock,
        string? manufacturer,
        string? externalRegistrationNumber)
    {
        Name = NormalizeRequiredText(name, nameof(name), 200);
        Category = category;
        MeasurementUnit = measurementUnit;
        MinimumStock = ValidateNonNegative(minimumStock, nameof(minimumStock));
        Manufacturer = NormalizeOptionalText(manufacturer, 200);
        ExternalRegistrationNumber = NormalizeOptionalText(externalRegistrationNumber, 100);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string NormalizeRequiredText(string value, string parameterName, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("O valor é obrigatório.", parameterName);
        }

        var normalized = value.Trim();
        if (normalized.Length > maximumLength)
        {
            throw new ArgumentException($"O valor deve possuir no máximo {maximumLength} caracteres.", parameterName);
        }

        return normalized;
    }

    private static string? NormalizeOptionalText(string? value, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > maximumLength)
        {
            throw new ArgumentException($"O valor deve possuir no máximo {maximumLength} caracteres.");
        }

        return normalized;
    }

    private static decimal ValidateNonNegative(decimal value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "O valor não pode ser negativo.");
        }

        return value;
    }
}
