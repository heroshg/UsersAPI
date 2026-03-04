using NetDevPack.SimpleMediator;
using System.ComponentModel.DataAnnotations;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Commands.ChangeUserRole
{
    public record ChangeUserRoleCommand(
        [Required(ErrorMessage = "User id is required")]
        Guid Id,
        [Required(ErrorMessage = "Role is required")]
        string Role
    ) : IRequest<ResultViewModel<UserAdminViewModel>>;
}
