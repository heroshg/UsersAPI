using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Commands.ChangeUserRole;

public record ChangeUserRoleCommand(Guid Id, string Role) : IRequest<ResultViewModel<UserViewModel>>;
