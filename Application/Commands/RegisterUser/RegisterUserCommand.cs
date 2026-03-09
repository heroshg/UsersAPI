using MediatR;
using UsersAPI.Application.Models;
using System.ComponentModel.DataAnnotations;

namespace UsersAPI.Application.Commands.RegisterUser;

public record RegisterUserCommand(
    [Required][MaxLength(150)] string Name,
    [Required][EmailAddress] string Email,
    [Required][MinLength(8)] string Password
) : IRequest<ResultViewModel<Guid>>;
