using NetDevPack.SimpleMediator;
using UsersAPI.Application.Models;
using UsersAPI.Domain.UserAggregate;

namespace UsersAPI.Application.Queries.GetUserByEmail
{
    public class GetUserByEmailHandler : IRequestHandler<GetUserByEmailQuery, ResultViewModel<UserAdminViewModel>>
    {
        private readonly IUserRepository _repository;

        public GetUserByEmailHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultViewModel<UserAdminViewModel>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var email = new Email(request.Email);

            var user = await _repository.GetByEmailAsync(email, cancellationToken);

            if (user is null)
                return ResultViewModel<UserAdminViewModel>.Error("User not found.");

            return ResultViewModel<UserAdminViewModel>.Success(UserAdminViewModel.FromEntity(user));
        }
    }

}
