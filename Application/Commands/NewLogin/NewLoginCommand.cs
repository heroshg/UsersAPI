using MediatR;
using UsersAPI.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace UsersAPI.Application.Commands.NewLogin;

public record NewLoginCommand(
    [Required][EmailAddress] string Email,
    [Required] string Password
) : IRequest<ResultViewModel<LoginViewModel>>;
