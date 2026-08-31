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
        var normalizedName = Guard.AgainstNullOrWhiteSpace(name, nameof(name), 150);
        var normalizedRegistration = string.IsNullOrWhiteSpace(registrationNumber)
            ? null
            : registrationNumber.Trim();

        return new Manufacturer(Guid.NewGuid(), normalizedName, normalizedRegistration);
    }

    public void Rename(string name) => Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name), 150);
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
