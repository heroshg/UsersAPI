using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Users.Infrastructure.Auth;

namespace Users.API.Controllers;

/// <summary>
/// Endpoints OIDC/JWKS para que o AWS API Gateway (e qualquer outro consumidor)
/// possa descobrir a chave pública usada para validar os JWTs emitidos por
/// esta API. Ambos os endpoints são públicos por design.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route(".well-known")]
public class WellKnownController(IConfiguration configuration) : ControllerBase
{
    [HttpGet("openid-configuration")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult OpenIdConfiguration()
    {
        var issuer = configuration["Jwt:Issuer"] ?? "FiapCloudGames";
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        return Ok(new
        {
            issuer,
            jwks_uri                            = $"{baseUrl}/.well-known/jwks.json",
            id_token_signing_alg_values_supported = new[] { "RS256" },
            response_types_supported            = new[] { "id_token" },
            subject_types_supported             = new[] { "public" }
        });
    }

    [HttpGet("jwks.json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Jwks()
    {
        var publicKeyB64 = configuration["Jwt:RsaPublicKey"]
            ?? throw new InvalidOperationException("Jwt:RsaPublicKey is missing.");

        using var rsa = RSA.Create();
        rsa.ImportFromPem(Encoding.UTF8.GetString(Convert.FromBase64String(publicKeyB64)));

        var parameters = rsa.ExportParameters(false);

        return Ok(new
        {
            keys = new[]
            {
                new
                {
                    kty = "RSA",
                    use = "sig",
                    alg = "RS256",
                    kid = JwtKeys.KeyId,
                    n   = Base64UrlEncoder.Encode(parameters.Modulus!),
                    e   = Base64UrlEncoder.Encode(parameters.Exponent!)
                }
            }
        });
    }
}
