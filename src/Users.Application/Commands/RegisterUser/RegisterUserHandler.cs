using FiapCloudGames.Contracts.Events;
using MassTransit;
using MediatR;
using Users.Application.DTOs;
using Users.Application.IntegrationEvents;
using Users.Domain.Entities;
using Users.Domain.Interfaces;
using Users.Domain.ValueObjects;

namespace Users.Application.Commands.RegisterUser;

public class RegisterUserHandler(
    IUserRepository repository,
    IPasswordHasher passwordHasher,
    IPublishEndpoint publishEndpoint,
    IEventPublisher eventPublisher)
    : IRequestHandler<RegisterUserCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        if (await repository.IsEmailRegisteredAsync(request.Email, ct))
            return ResultViewModel<Guid>.Error("Email already in use.");

        Password.FromPlainText(request.Password);
        var hash = passwordHasher.HashPassword(request.Password);

        var user = User.Create(
            request.Name,
            new Email(request.Email),
            Password.FromHash(hash));

        var id = await repository.AddAsync(user, ct);

        var evt = new UserCreatedEvent(id, user.Name.Value, request.Email);

        await publishEndpoint.Publish(evt, ct);

        await eventPublisher.PublishUserCreatedAsync(evt, ct);

        return ResultViewModel<Guid>.Success(id);
    }
}
