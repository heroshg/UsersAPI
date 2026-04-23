using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
using Users.Domain.Interfaces;

namespace Users.Infrastructure.Identity;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 3;
    private const int MemorySize = 1024 * 4; // 4 MB — safe for t2.micro
    private const int Parallelism = 1;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            Iterations = Iterations,
            MemorySize = MemorySize,
            DegreeOfParallelism = Parallelism
        };
        var hash = argon2.GetBytes(HashSize);
        return $"argon2id.{Iterations}.{MemorySize}.{Parallelism}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.');
        if (parts.Length != 6 || parts[0] != "argon2id") return false;

        var iterations = int.Parse(parts[1]);
        var memory = int.Parse(parts[2]);
        var parallelism = int.Parse(parts[3]);
        var salt = Convert.FromBase64String(parts[4]);
        var expected = Convert.FromBase64String(parts[5]);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            Iterations = iterations,
            MemorySize = memory,
            DegreeOfParallelism = parallelism
        };

        var computed = argon2.GetBytes(expected.Length);
        return CryptographicOperations.FixedTimeEquals(computed, expected);
    }
}
