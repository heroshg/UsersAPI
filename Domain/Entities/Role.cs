namespace UsersAPI.Domain.Entities;

public class Role
{
    public static readonly Role User = new("User");
    public static readonly Role Admin = new("Admin");

    public string Value { get; }

    protected Role() { Value = string.Empty; }

    private Role(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Role is required.");
        Value = value;
    }

    public static Role From(string value)
    {
        if (string.Equals(value, User.Value, StringComparison.OrdinalIgnoreCase)) return User;
        if (string.Equals(value, Admin.Value, StringComparison.OrdinalIgnoreCase)) return Admin;
        throw new DomainException("Invalid role.");
    }
}
