using UsersAPI.Domain.Common;

namespace UsersAPI.Domain.UserAggregate
{
    public class Role
    {
        public static readonly Role User = new("User");
        public static readonly Role Admin = new("Admin");

        public string Value { get; }

        protected Role() { }

        private Role(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Role is required.");

            Value = value;
        }
    }
}
