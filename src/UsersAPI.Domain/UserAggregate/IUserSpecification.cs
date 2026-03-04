namespace UsersAPI.Domain.UserAggregate
{
    public interface IUserSpecification
    {
        Task<bool> IsSatisfiedByAsync(Email email, CancellationToken ct);
    }
}
