using MediatR;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Queries.GetUserByEmail;

public record GetUserByEmailQuery(string Email) : IRequest<ResultViewModel<UserViewModel>>;
