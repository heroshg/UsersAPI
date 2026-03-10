using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<ResultViewModel<bool>>;
