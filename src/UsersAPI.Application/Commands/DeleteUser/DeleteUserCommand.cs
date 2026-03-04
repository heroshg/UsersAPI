using NetDevPack.SimpleMediator;
using System.ComponentModel.DataAnnotations;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Commands.DeleteUser
{
    public record DeleteUserCommand(
    [Required(ErrorMessage = "User id is required")]
    Guid Id
        ) : IRequest<ResultViewModel<UserAdminViewModel>>;
}
