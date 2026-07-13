using Amazon.SQS;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.IntegrationEvents;
using Users.Domain.Interfaces;
using Users.Infrastructure.Auth;
using Users.Infrastructure.Identity;
using Users.Infrastructure.Messaging;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Persistence.Repositories;
using Users.Infrastructure.Persistence.Seed;

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
        services.AddScoped<UsersDbSeeder>();

        var redisConnection = configuration["Redis:ConnectionString"];
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(opts =>
            {
                opts.Configuration = redisConnection;
                opts.InstanceName   = "fcg-users:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        var region = configuration["AWS:Region"];
        if (!string.IsNullOrWhiteSpace(region))
        {
            services.AddSingleton<IAmazonSQS>(_ =>
                new AmazonSQSClient(Amazon.RegionEndpoint.GetBySystemName(region)));
            services.AddSingleton<ISqsPublisher, SqsPublisher>();
        }
        else
        {
            services.AddSingleton<ISqsPublisher, NoopSqsPublisher>();
        }
        services.AddScoped<IEventPublisher, SqsEventPublisher>();

        services.AddMassTransit(x =>
        {
            x.DisableUsageTelemetry();
            x.UsingAmazonSqs((ctx, cfg) =>
            {
                // Credenciais via cadeia padrão da AWS (IAM): perfil local / node role (LabRole) no EKS.
                cfg.Host(configuration["AWS:Region"] ?? "us-east-1", h =>
                {
                    var scope = configuration["Messaging:Scope"];
                    if (!string.IsNullOrWhiteSpace(scope))
                        h.Scope(scope, true);
                });
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
