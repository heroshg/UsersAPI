using Users.Domain.Exceptions;
using Users.Domain.ValueObjects;

namespace Users.Domain.Entities;

public class User : Entity
{
    protected User() { }

    public static User Create(string name, Email email, Password password)
    {
        return new User
        {
            Name     = UserName.From(name),
            Email    = email    ?? throw new DomainException("Email is required."),
            Password = password ?? throw new DomainException("Password is required."),
            Role     = Role.User
        };
    }

    public UserName Name     { get; private set; } = null!;
    public Email    Email    { get; private set; } = null!;
    public Password Password { get; private set; } = null!;
    public Role     Role     { get; private set; } = null!;

    public void ChangeName(string name)
    {
        Name = UserName.From(name);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeEmail(Email newEmail)
    {
        Email = newEmail ?? throw new DomainException("Email is required.");
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(string role)
    {
        Role = Role.From(role);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(Password newPassword)
    {
        Password = newPassword ?? throw new DomainException("Password is required.");
        UpdatedAt = DateTime.UtcNow;
    }
}
