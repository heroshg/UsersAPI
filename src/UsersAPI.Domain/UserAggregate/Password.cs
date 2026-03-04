using UsersAPI.Domain.Common;

namespace UsersAPI.Domain.UserAggregate
{
    public class Password
    {
        public string Value { get; }

        private Password(string value)
        {
            Value = value;
        }

        public static Password FromPlainText(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                throw new DomainException("Password is required.");

            if (plainText.Length < 8)
                throw new DomainException("Password must be at least 8 characters.");

            if (!plainText.Any(char.IsLetter))
                throw new DomainException("Password must contain a letter.");

            if (!plainText.Any(char.IsDigit))
                throw new DomainException("Password must contain a digit.");

            if (!plainText.Any(c => !char.IsLetterOrDigit(c)))
                throw new DomainException("Password must contain a special character.");
            return new Password(plainText);
        }

        public static Password FromHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new DomainException("Password hash cannot be empty.");

            return new Password(hash);
        }
    }
}
