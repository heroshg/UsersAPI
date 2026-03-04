using NetDevPack.SimpleMediator;
using UsersAPI.Application.Models;
using UsersAPI.Domain.UserAggregate;

namespace UsersAPI.Application.Queries.GetUserById
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, ResultViewModel<UserAdminViewModel>>
    {
        private readonly IUserRepository _repository;

        public GetUserByIdHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultViewModel<UserAdminViewModel>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (user is null)
                return ResultViewModel<UserAdminViewModel>.Error("User not found.");

            return ResultViewModel<UserAdminViewModel>.Success(UserAdminViewModel.FromEntity(user));
        }
    }
}
