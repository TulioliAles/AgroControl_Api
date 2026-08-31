using AgroControl.Domain.Common;

namespace AgroControl.Domain.Catalog;

public sealed class Manufacturer : Entity
{
    private Manufacturer(Guid id, string name, string? registrationNumber)
        : base(id)
    {
        Name = name;
        RegistrationNumber = registrationNumber;
        IsActive = true;
    }

    public string Name { get; private set; }
    public string? RegistrationNumber { get; private set; }
    public bool IsActive { get; private set; }

    public static Manufacturer Create(string name, string? registrationNumber = null)
    {
        var manufacturer = new Manufacturer(Guid.NewGuid(), string.Empty, null);
        manufacturer.Update(name, registrationNumber);
        return manufacturer;
    }

    public void Update(string name, string? registrationNumber)
    {
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name), 150);
        RegistrationNumber = string.IsNullOrWhiteSpace(registrationNumber)
            ? null
            : registrationNumber.Trim();
    }

    public void Rename(string name) => Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name), 150);
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
