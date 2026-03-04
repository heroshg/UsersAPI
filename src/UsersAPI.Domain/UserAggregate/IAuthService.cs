namespace UsersAPI.Domain.UserAggregate
{
    public interface IAuthService
    {
        string GenerateToken(Guid userId, string email, string role);
    }
}
