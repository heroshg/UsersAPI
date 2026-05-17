using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Users.Domain.Interfaces;

namespace Users.Infrastructure.Auth;

public class AuthService(IConfiguration configuration) : IAuthService
{
    public string GenerateToken(Guid userId, string email, string role)
    {
        var rsa = LoadRsaPrivateKey(configuration);

        var credentials = new SigningCredentials(
            new RsaSecurityKey(rsa) { KeyId = JwtKeys.KeyId },
            SecurityAlgorithms.RsaSha256);

        var claims = new List<Claim>
        {
            new("userId", userId.ToString()),
            new("username", email),
            new(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static RSA LoadRsaPrivateKey(IConfiguration configuration)
    {
        var b64 = configuration["Jwt:RsaPrivateKey"]
            ?? throw new InvalidOperationException("Jwt:RsaPrivateKey is missing.");

        var pem = Encoding.UTF8.GetString(Convert.FromBase64String(b64));
        var rsa = RSA.Create();
        rsa.ImportFromPem(pem);
        return rsa;
    }
}
