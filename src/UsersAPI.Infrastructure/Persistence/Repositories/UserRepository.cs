using Microsoft.EntityFrameworkCore;
using UsersAPI.Domain.UserAggregate;

namespace UsersAPI.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FiapCloudGamesUsersDbContext _context;

        public UserRepository(FiapCloudGamesUsersDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddAsync(User user, CancellationToken cancellationToken)
        {
            await _context.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return user.Id;
        }

        public async Task<bool> ExistsById(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
        {
            return await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email.Address == email.Address, cancellationToken);
        }
        public async Task<List<User>> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            var normalizedName = name.Trim().ToLower();

            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Name.ToLower().Contains(normalizedName))
                .OrderBy(u => u.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.Address == email);
        }

        public async Task<(List<User> Items, int TotalCount)> ListPagedAsync(
            bool includeInactive,
            int skip,
            int take,
            CancellationToken cancellationToken)
        {
            IQueryable<User> query = _context.Users;

            if (!includeInactive)
                query = query.Where(u => u.IsActive);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(u => u.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
