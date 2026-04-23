using Users.Domain.Exceptions;

namespace Users.Domain.ValueObjects;

public sealed class UserName
{
    public const int MaxLength = 150;

    public string Value { get; }

    private UserName(string value) => Value = value;

    public static UserName From(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("User name cannot be null or empty.");
        var normalized = name.Trim();
        if (normalized.Length > MaxLength)
            throw new DomainException($"User name cannot exceed {MaxLength} characters.");
        return new UserName(normalized);
    }

    public override bool Equals(object? obj) => obj is UserName n && Value == n.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(UserName? a, UserName? b) => a?.Value == b?.Value;
    public static bool operator !=(UserName? a, UserName? b) => !(a == b);
    public override string ToString() => Value;
}
