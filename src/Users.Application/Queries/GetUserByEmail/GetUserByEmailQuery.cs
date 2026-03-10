using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Queries.GetUserByEmail;

public record GetUserByEmailQuery(string Email) : IRequest<ResultViewModel<UserViewModel>>;
