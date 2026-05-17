namespace Users.Infrastructure.Auth;

/// <summary>
/// Identificadores compartilhados entre quem assina (AuthService) e quem
/// publica a chave pública (WellKnownController). O <c>KeyId</c> aparece
/// no header <c>kid</c> do JWT e no campo correspondente do JWKS,
/// permitindo que o validador (API Gateway, outras APIs) saiba qual chave
/// pública usar para verificar a assinatura.
/// </summary>
public static class JwtKeys
{
    public const string KeyId = "fcg-rsa-1";
}
