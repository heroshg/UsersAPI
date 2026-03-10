using MediatR;
using Users.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Users.Application.Commands.NewLogin;

public record NewLoginCommand(
    [Required][EmailAddress] string Email,
    [Required] string Password
) : IRequest<ResultViewModel<LoginViewModel>>;
