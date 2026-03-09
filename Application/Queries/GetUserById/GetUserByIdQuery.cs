using MediatR;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<ResultViewModel<UserViewModel>>;
