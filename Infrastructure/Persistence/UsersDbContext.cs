using Microsoft.EntityFrameworkCore;
using UsersAPI.Domain.Entities;
using UsersAPI.Infrastructure.Persistence.Configurations;

namespace UsersAPI.Infrastructure.Persistence;

public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
