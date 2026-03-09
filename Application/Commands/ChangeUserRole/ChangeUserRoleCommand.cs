using MediatR;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Commands.ChangeUserRole;

public record ChangeUserRoleCommand(Guid Id, string Role) : IRequest<ResultViewModel<UserViewModel>>;
