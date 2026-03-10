using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<ResultViewModel<UserViewModel>>;
