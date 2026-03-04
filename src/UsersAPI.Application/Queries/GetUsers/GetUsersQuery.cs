using NetDevPack.SimpleMediator;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Queries.GetUsers
{
    public record GetUsersQuery(
        int Page = 1,
        int PageSize = 5,
        bool IncludeInactive = false
    ) : IRequest<ResultViewModel<PagedResultViewModel<UserAdminViewModel>>>;
}
