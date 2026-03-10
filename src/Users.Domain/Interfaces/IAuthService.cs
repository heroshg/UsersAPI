namespace Users.Domain.Interfaces;

public interface IAuthService
{
    string GenerateToken(Guid userId, string email, string role);
}
