using FiapCloudGames.Contracts.Events;
using MassTransit;
using MediatR;
using UsersAPI.Application.Models;
using UsersAPI.Domain.Entities;
using UsersAPI.Domain.Interfaces;

namespace UsersAPI.Application.Commands.RegisterUser;

public class RegisterUserHandler(
    IUserRepository repository,
    IPasswordHasher passwordHasher,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<RegisterUserCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        if (await repository.IsEmailRegisteredAsync(request.Email, ct))
            return ResultViewModel<Guid>.Error("Email already in use.");

        Password.FromPlainText(request.Password); // validates rules
        var hash = passwordHasher.HashPassword(request.Password);

        var user = User.Create(
            request.Name,
            new Email(request.Email),
            Password.FromHash(hash));

        var id = await repository.AddAsync(user, ct);

        await publishEndpoint.Publish(new UserCreatedEvent(id, user.Name, request.Email), ct);

        return ResultViewModel<Guid>.Success(id);
    }
}
