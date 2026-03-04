using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UsersAPI.Domain.Common;
using UsersAPI.Domain.UserAggregate;
using UsersAPI.Infrastructure.Auth;
using UsersAPI.Infrastructure.Identity;
using UsersAPI.Infrastructure.Logging;
using UsersAPI.Infrastructure.Persistence;
using UsersAPI.Infrastructure.Persistence.Repositories;

namespace UsersAPI.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddFiapCloudGamesContext(configuration);
            services.AddRepositoriesDependencies();
            services.AddAuth(configuration);
            services.AddCorrelationIdGenerator();
            services.AddLogger();
            return services;
        }

        private static IServiceCollection AddFiapCloudGamesContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("FiapCloudGames");
            services.AddDbContext<FiapCloudGamesUsersDbContext>(opts =>
            {
                opts.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
            return services;
        }

        private static IServiceCollection AddRepositoriesDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }

        private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new NullReferenceException("The jwt key is null.")))
                    };
                });
            return services;
        }

        private static IServiceCollection AddCorrelationIdGenerator(this IServiceCollection services)
        {
            services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
            return services;
        }

        private static IServiceCollection AddLogger(this IServiceCollection services)
        {
            services.AddScoped(typeof(BaseLogger<>));
            return services;
        }
    }
}
