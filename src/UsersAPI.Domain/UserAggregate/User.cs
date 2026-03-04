using System.Data;
using UsersAPI.Domain.Common;

namespace UsersAPI.Domain.UserAggregate
{
    public class User : AggregateRoot
    {
        private const int MaxNameLength = 150;

        // EF Core
        public User()
        {

        }

        public static User Create(string name, Email email, Password password)
        {
            var validatedName = ValidateAndNormalizeName(name);

            return new User
            {
                Name = validatedName,
                Email = email ?? throw new DomainException("Email is required."),
                Password = password ?? throw new DomainException("Password is required."),
                Role = Role.User,
                Balance = 0m
            };

        }



        public Email Email { get; private set; }
        public Password Password { get; private set; }
        public Role Role { get; private set; }
        public string Name { get; private set; }
        public decimal Balance { get; private set; }
        public void ChangeName(string name)
        {
            Name = ValidateAndNormalizeName(name);
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeEmail(Email newEmail, bool emailAlreadyExists)
        {
            Email = newEmail ?? throw new DomainException("Email is required.");
            UpdatedAt = DateTime.UtcNow;
        }


        private static string ValidateAndNormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("User name cannot be null or empty.");

            var normalized = name.Trim();

            if (normalized.Length > MaxNameLength)
                throw new DomainException("Name is too long.");

            return normalized;
        }

        public void Debit(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Amount must be greater than zero.");
            if (Balance < amount)
                throw new DomainException("Insufficient balance.");
            Balance -= amount;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool CanAfford(decimal amount)
            => Balance >= amount;

        public void ChangeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new DomainException("Role cannot be null or empty.");

            if (string.Equals(role, Role.User.Value, StringComparison.OrdinalIgnoreCase))
                Role = Role.User;
            else if (string.Equals(role, Role.Admin.Value, StringComparison.OrdinalIgnoreCase))
                Role = Role.Admin;
            else
                throw new DomainException("Invalid role.");

            UpdatedAt = DateTime.UtcNow;
        }
    }
}
