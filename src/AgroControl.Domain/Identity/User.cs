using AgroControl.Domain.Common;

namespace AgroControl.Domain.Identity;

public sealed class User : Entity
{
    private User(Guid id, string name, string email, string passwordHash, string role)
        : base(id)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
    }

    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }
    public bool IsActive { get; private set; }

    public static User Create(
        string name,
        string email,
        string passwordHash,
        string role = "Admin")
    {
        var normalizedName = Guard.AgainstNullOrWhiteSpace(name, nameof(name), 150);
        var normalizedEmail = Guard.AgainstNullOrWhiteSpace(email, nameof(email), 254)
            .ToLowerInvariant();
        var normalizedHash = Guard.AgainstNullOrWhiteSpace(passwordHash, nameof(passwordHash), 1000);
        var normalizedRole = Guard.AgainstNullOrWhiteSpace(role, nameof(role), 50);

        if (!normalizedEmail.Contains('@'))
        {
            throw new ArgumentException("A valid email address is required.", nameof(email));
        }

        return new User(
            Guid.NewGuid(),
            normalizedName,
            normalizedEmail,
            normalizedHash,
            normalizedRole);
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
