using NetDevPack.SimpleMediator;
using UsersAPI.Application.Models;
using UsersAPI.Domain.UserAggregate;

namespace UsersAPI.Application.Queries.GetUserByName
{
    public class GetUserByNameHandler : IRequestHandler<GetUserByNameQuery, ResultViewModel<List<UserAdminViewModel>>>
    {
        private readonly IUserRepository _repository;

        public GetUserByNameHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultViewModel<List<UserAdminViewModel>>> Handle(GetUserByNameQuery request, CancellationToken cancellationToken)
        {

            var users = await _repository.GetByNameAsync(request.Name, cancellationToken);

            if (users.Count == 0)
                return ResultViewModel<List<UserAdminViewModel>>.Error("No users found.");

            var result = users.Select(UserAdminViewModel.FromEntity).ToList();

            return ResultViewModel<List<UserAdminViewModel>>.Success(result);
        }
    }
}
