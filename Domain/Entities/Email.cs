namespace UsersAPI.Domain.Entities;

public class Email
{
    public string Address { get; private set; }

    protected Email() { Address = string.Empty; }

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Email cannot be empty.");
        Address = address;
    }
}
