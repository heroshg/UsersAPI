using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Users.Domain.Entities;
using Users.Domain.Interfaces;
using Users.Domain.ValueObjects;

namespace Users.Infrastructure.Persistence.Seed;

public class UsersDbSeeder(
    UsersDbContext db,
    IPasswordHasher passwordHasher,
    ILogger<UsersDbSeeder> logger)
{
    private const string AdminEmail    = "admin@fgc.com";
    private const string AdminName     = "Admin";
    private const string AdminPassword = "Admin@123";

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await SeedAdminAsync(ct);
    }

    private async Task SeedAdminAsync(CancellationToken ct)
    {
        if (await db.Users.AnyAsync(u => u.Email.Address == AdminEmail, ct))
        {
            logger.LogInformation("Admin seed skipped — user already exists ({Email})", AdminEmail);
            return;
        }

        Password.FromPlainText(AdminPassword);
        var hash = passwordHasher.HashPassword(AdminPassword);

        var admin = User.Create(AdminName, new Email(AdminEmail), Password.FromHash(hash));
        admin.ChangeRole(Role.Admin.Value);

        db.Users.Add(admin);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Admin user seeded ({Email})", AdminEmail);
    }
}
