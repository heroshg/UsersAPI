using MediatR;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<ResultViewModel<bool>>;
