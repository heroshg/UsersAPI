using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, string? Name, string? Email, bool? IsActive)
    : IRequest<ResultViewModel<UserViewModel>>;
