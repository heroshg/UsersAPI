using Microsoft.Extensions.DependencyInjection;
using NetDevPack.SimpleMediator;
using UsersAPI.Application.Commands.RegisterUser;
using UsersAPI.Application.Models;

namespace UsersAPI.Application
{
    public static class ApplicationModule
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediator();
            return services;
        }
        private static IServiceCollection AddMediator(this IServiceCollection services)
        {
            services.AddSimpleMediator();
            services.AddScoped<IMediator, Mediator>();

            services.AddScoped<
                IRequestHandler<RegisterUserCommand, ResultViewModel<Guid>>,
                RegisterUserHandler>();
            return services;
        }
    }
}
