using NetDevPack.SimpleMediator;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Queries.GetUserById
{
    public record GetUserByIdQuery(Guid Id) : IRequest<ResultViewModel<UserAdminViewModel>>;
}
