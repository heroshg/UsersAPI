using Users.Domain.Exceptions;

namespace Users.Domain.ValueObjects;

public class Email
{
    public string Address { get; private set; }

    protected Email() { Address = string.Empty; }

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Email cannot be empty.");
        var normalized = address.Trim().ToLowerInvariant();
        if (!System.Net.Mail.MailAddress.TryCreate(normalized, out _))
            throw new DomainException("Email format is invalid.");
        Address = normalized;
    }

    public override bool Equals(object? obj) => obj is Email e && Address == e.Address;
    public override int GetHashCode() => Address.GetHashCode();
    public static bool operator ==(Email? a, Email? b) => a?.Address == b?.Address;
    public static bool operator !=(Email? a, Email? b) => !(a == b);
    public override string ToString() => Address;
}
