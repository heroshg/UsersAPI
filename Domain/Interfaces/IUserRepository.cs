using UsersAPI.Domain.Entities;

namespace UsersAPI.Domain.Interfaces;

public interface IUserRepository
{
    Task<Guid> AddAsync(User user, CancellationToken ct);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<List<User>> GetByNameAsync(string name, CancellationToken ct);
    Task<(List<User> Items, int Total)> ListPagedAsync(bool includeInactive, int skip, int take, CancellationToken ct);
    Task<bool> IsEmailRegisteredAsync(string email, CancellationToken ct);
    Task UpdateAsync(User user, CancellationToken ct);
    Task DeleteAsync(User user, CancellationToken ct);
}
