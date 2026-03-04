using NetDevPack.SimpleMediator;
using System.ComponentModel.DataAnnotations;
using UsersAPI.Application.Models;
using UsersAPI.Domain.UserAggregate;

namespace UsersAPI.Application.Commands.DeleteUser
{
    public class DeleteUserHandler
        : IRequestHandler<DeleteUserCommand, ResultViewModel<UserAdminViewModel>>
    {
        private readonly IUserRepository _repository;

        public DeleteUserHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultViewModel<UserAdminViewModel>> Handle(
            DeleteUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (user is null)
                return ResultViewModel<UserAdminViewModel>.Error("User not found.");

            if (!user.IsActive)
                return ResultViewModel<UserAdminViewModel>.Error("User is already inactive.");

            user.Deactivate();

            await _repository.UpdateAsync(user, cancellationToken);

            return ResultViewModel<UserAdminViewModel>.Success(
                UserAdminViewModel.FromEntity(user)
            );
        }
    }
}
