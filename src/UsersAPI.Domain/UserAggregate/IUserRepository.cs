namespace UsersAPI.Domain.UserAggregate
{
    public interface IUserRepository
    {
        Task<bool> IsEmailRegisteredAsync(string email);
        Task<Guid> AddAsync(User user, CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken);
        Task<List<User>> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task<bool> ExistsById(Guid id, CancellationToken cancellationToken);
        Task<(List<User> Items, int TotalCount)> ListPagedAsync(bool includeInactive, int skip,int take,CancellationToken cancellationToken);
        Task UpdateAsync(User user, CancellationToken cancellationToken);
    }
}
