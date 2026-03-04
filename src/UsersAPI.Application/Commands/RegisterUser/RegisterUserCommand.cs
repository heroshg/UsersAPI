using NetDevPack.SimpleMediator;
using System.ComponentModel.DataAnnotations;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Commands.RegisterUser
{
    public record RegisterUserCommand(
        [Required]
        [MaxLength(80)]
        string Name,
        [Required]
        [EmailAddress]
        string Email,
        [Required]
        [MinLength(8)]
        string Password) : IRequest<ResultViewModel<Guid>>
    {
    }
}
