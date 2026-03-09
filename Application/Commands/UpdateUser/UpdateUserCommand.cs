using MediatR;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, string? Name, string? Email, bool? IsActive)
    : IRequest<ResultViewModel<UserViewModel>>;
