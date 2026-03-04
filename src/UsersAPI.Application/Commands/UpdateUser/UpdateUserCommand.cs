using NetDevPack.SimpleMediator;
using System.ComponentModel.DataAnnotations;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Commands.UpdateUser
{
    public record UpdateUserCommand(
        [Required]
        Guid Id,
        string? Name = null,
        string? Email = null,
        bool? IsActive = null
    ) : IRequest<ResultViewModel<UserAdminViewModel>>;
}
