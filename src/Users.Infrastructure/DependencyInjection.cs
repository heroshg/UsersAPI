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

namespace Users.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // ── Banco de Dados ────────────────────────────────────────────────────
        services.AddDbContext<UsersDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("Users")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();

        // ── Cache — Redis ─────────────────────────────────────────────────────
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

        // ── Mensageria — SQS (produção AWS) ───────────────────────────────────
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

        // ── Mensageria — RabbitMQ (desenvolvimento local) ─────────────────────
        services.AddMassTransit(x =>
        {
            x.DisableUsageTelemetry();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? throw new InvalidOperationException("RabbitMQ:Username is missing."));
                    h.Password(configuration["RabbitMQ:Password"] ?? throw new InvalidOperationException("RabbitMQ:Password is missing."));
                });
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
