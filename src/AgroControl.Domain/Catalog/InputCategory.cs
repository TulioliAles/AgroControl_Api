using AgroControl.Domain.Common;

namespace AgroControl.Domain.Catalog;

public sealed class InputCategory : Entity
{
    private InputCategory(Guid id, string name, string? description)
        : base(id)
    {
        Name = name;
        Description = description;
        IsActive = true;
    }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    public static InputCategory Create(string name, string? description = null)
    {
        var normalizedName = Guard.AgainstNullOrWhiteSpace(name, nameof(name), 100);
        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        return new InputCategory(Guid.NewGuid(), normalizedName, normalizedDescription);
    }

    public void Rename(string name) => Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name), 100);
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
