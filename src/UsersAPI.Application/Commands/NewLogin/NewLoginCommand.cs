using NetDevPack.SimpleMediator;
using System.ComponentModel.DataAnnotations;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Commands.NewLogin
{
    public record NewLoginCommand(
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        string Email,
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password length must be greater or equal to 8")]
        string Password) : IRequest<ResultViewModel<LoginViewModel>>
    {
    }
}
