using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UsersAPI.Domain.UserAggregate;
using UsersAPI.Infrastructure.Persistence.Configurations;

namespace UsersAPI.Infrastructure.Persistence
{
    public class FiapCloudGamesUsersDbContext : DbContext
    {
        public FiapCloudGamesUsersDbContext(DbContextOptions<FiapCloudGamesUsersDbContext> opts) : base(opts)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }

        public DbSet<User> Users { get; set; }
    }
}
