using MediatR;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Queries.GetUsers;

public record GetUsersQuery(int Page = 0, int PageSize = 10, bool IncludeInactive = false)
    : IRequest<ResultViewModel<PageResultViewModel<UserViewModel>>>;
