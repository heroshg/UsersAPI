using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Domain.Interfaces;
using Users.Infrastructure.Auth;
using Users.Infrastructure.Identity;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Persistence.Repositories;

namespace Users.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("Users")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
