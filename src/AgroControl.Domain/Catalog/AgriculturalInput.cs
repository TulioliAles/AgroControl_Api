using AgroControl.Domain.Catalog.Events;
using AgroControl.Domain.Common;

namespace AgroControl.Domain.Catalog;

public sealed class AgriculturalInput : AggregateRoot
{
    private AgriculturalInput(
        Guid id,
        string name,
        string? commercialName,
        AgriculturalInputType type,
        Guid categoryId,
        Guid manufacturerId,
        Guid measurementUnitId)
        : base(id)
    {
        Name = name;
        CommercialName = commercialName;
        Type = type;
        CategoryId = categoryId;
        ManufacturerId = manufacturerId;
        MeasurementUnitId = measurementUnitId;
        IsActive = true;
    }

    public string Name { get; private set; }
    public string? CommercialName { get; private set; }
    public AgriculturalInputType Type { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid ManufacturerId { get; private set; }
    public Guid MeasurementUnitId { get; private set; }
    public bool IsActive { get; private set; }

    public static AgriculturalInput Create(
        string name,
        string? commercialName,
        AgriculturalInputType type,
        Guid categoryId,
        Guid manufacturerId,
        Guid measurementUnitId)
    {
        var input = new AgriculturalInput(
            Guid.NewGuid(),
            Guard.AgainstNullOrWhiteSpace(name, nameof(name), 150),
            NormalizeOptionalText(commercialName, 150, nameof(commercialName)),
            type,
            Guard.AgainstEmpty(categoryId, nameof(categoryId)),
            Guard.AgainstEmpty(manufacturerId, nameof(manufacturerId)),
            Guard.AgainstEmpty(measurementUnitId, nameof(measurementUnitId)));

        input.RaiseDomainEvent(new AgriculturalInputCreatedDomainEvent(input.Id, input.Name));
        return input;
    }

    public void Update(
        string name,
        string? commercialName,
        AgriculturalInputType type,
        Guid categoryId,
        Guid manufacturerId,
        Guid measurementUnitId)
    {
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name), 150);
        CommercialName = NormalizeOptionalText(commercialName, 150, nameof(commercialName));
        Type = type;
        CategoryId = Guard.AgainstEmpty(categoryId, nameof(categoryId));
        ManufacturerId = Guard.AgainstEmpty(manufacturerId, nameof(manufacturerId));
        MeasurementUnitId = Guard.AgainstEmpty(measurementUnitId, nameof(measurementUnitId));

        RaiseDomainEvent(new AgriculturalInputUpdatedDomainEvent(Id, Name));
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    private static string? NormalizeOptionalText(string? value, int maximumLength, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalizedValue = value.Trim();
        if (normalizedValue.Length > maximumLength)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                normalizedValue.Length,
                $"The value cannot exceed {maximumLength} characters.");
        }

        return normalizedValue;
    }
}
