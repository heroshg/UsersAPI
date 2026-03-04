using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
using UsersAPI.Domain.UserAggregate;

namespace UsersAPI.Infrastructure.Identity
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 4;
        private const int MemorySize = 1024 * 64;
        private const int DegreeOfParallelism = 2;

        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                Iterations = Iterations,
                MemorySize = MemorySize,
                DegreeOfParallelism = DegreeOfParallelism
            };

            byte[] hash = argon2.GetBytes(HashSize);

            var result = string.Join(".",
                "argon2id",
                Iterations,
                MemorySize,
                DegreeOfParallelism,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash)
            );

            return result;
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var parts = hashedPassword.Split('.');
            if (parts.Length != 6 || parts[0] != "argon2id")
                return false;

            int iterations = int.Parse(parts[1]);
            int memory = int.Parse(parts[2]);
            int parallelism = int.Parse(parts[3]);
            byte[] salt = Convert.FromBase64String(parts[4]);
            byte[] expectedHash = Convert.FromBase64String(parts[5]);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                Iterations = iterations,
                MemorySize = memory,
                DegreeOfParallelism = parallelism
            };

            byte[] computedHash = argon2.GetBytes(expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(
                computedHash,
                expectedHash
            );
        }
    }
}
