using Users.Domain.Exceptions;
using Users.Domain.ValueObjects;

namespace Users.Domain.Entities;

public class User : Entity
{
    private const int MaxNameLength = 150;

    protected User() { }

    public static User Create(string name, Email email, Password password)
    {
        var normalized = ValidateName(name);
        return new User
        {
            Name = normalized,
            Email = email ?? throw new DomainException("Email is required."),
            Password = password ?? throw new DomainException("Password is required."),
            Role = Role.User
        };
    }

    public Email Email { get; private set; } = null!;
    public Password Password { get; private set; } = null!;
    public Role Role { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;

    public void ChangeName(string name) { Name = ValidateName(name); UpdatedAt = DateTime.UtcNow; }
    public void ChangeEmail(Email newEmail) { Email = newEmail ?? throw new DomainException("Email is required."); UpdatedAt = DateTime.UtcNow; }
    public void ChangeRole(string role) { Role = Role.From(role); UpdatedAt = DateTime.UtcNow; }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("User name cannot be null or empty.");
        var normalized = name.Trim();
        if (normalized.Length > MaxNameLength)
            throw new DomainException("Name is too long.");
        return normalized;
    }
}
