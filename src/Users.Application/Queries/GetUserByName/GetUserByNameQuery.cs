using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Queries.GetUserByName;

public record GetUserByNameQuery(string Name) : IRequest<ResultViewModel<List<UserViewModel>>>;
