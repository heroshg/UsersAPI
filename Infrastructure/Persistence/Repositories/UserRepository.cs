using Microsoft.EntityFrameworkCore;
using UsersAPI.Domain.Entities;
using UsersAPI.Domain.Interfaces;

namespace UsersAPI.Infrastructure.Persistence.Repositories;

public class UserRepository(UsersDbContext context) : IUserRepository
{
    public async Task<Guid> AddAsync(User user, CancellationToken ct)
    {
        await context.Users.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        await context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email.Address == email, ct);

    public async Task<List<User>> GetByNameAsync(string name, CancellationToken ct)
    {
        var normalized = name.Trim().ToLower();
        return await context.Users
            .AsNoTracking()
            .Where(u => u.Name.ToLower().Contains(normalized))
            .OrderBy(u => u.Name)
            .ToListAsync(ct);
    }

    public async Task<(List<User> Items, int Total)> ListPagedAsync(bool includeInactive, int skip, int take, CancellationToken ct)
    {
        IQueryable<User> query = context.Users;
        if (!includeInactive)
            query = query.Where(u => u.IsActive);

        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(u => u.CreatedAt).Skip(skip).Take(take).ToListAsync(ct);
        return (items, total);
    }

    public async Task<bool> IsEmailRegisteredAsync(string email, CancellationToken ct) =>
        await context.Users.AnyAsync(u => u.Email.Address == email, ct);

    public async Task UpdateAsync(User user, CancellationToken ct)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(User user, CancellationToken ct)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync(ct);
    }
}
