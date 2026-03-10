using MediatR;
using Users.Application.DTOs;

namespace Users.Application.Queries.GetUsers;

public record GetUsersQuery(int Page = 0, int PageSize = 10, bool IncludeInactive = false)
    : IRequest<ResultViewModel<PageResultViewModel<UserViewModel>>>;
