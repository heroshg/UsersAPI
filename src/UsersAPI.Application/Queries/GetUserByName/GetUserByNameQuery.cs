using NetDevPack.SimpleMediator;
using UsersAPI.Application.Models;

namespace UsersAPI.Application.Queries.GetUserByName
{
    public record GetUserByNameQuery(string Name) : IRequest<ResultViewModel<List<UserAdminViewModel>>>;
}
