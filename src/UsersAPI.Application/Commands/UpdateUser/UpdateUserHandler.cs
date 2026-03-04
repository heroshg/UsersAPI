using NetDevPack.SimpleMediator;
using UsersAPI.Application.Models;
using UsersAPI.Domain.Common;
using UsersAPI.Domain.UserAggregate;

namespace UsersAPI.Application.Commands.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, ResultViewModel<UserAdminViewModel>>
    {
        private readonly IUserRepository _repository;
        private readonly IUserSpecification _specification;

        public UpdateUserHandler(IUserRepository repository, IUserSpecification specification)
        {
            _repository = repository;
            _specification = specification;
        }

        public async Task<ResultViewModel<UserAdminViewModel>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (user is null)
                return ResultViewModel<UserAdminViewModel>.Error("User not found.");

            var changed = false;

            if (request.Name is not null)
            {
                user.ChangeName(request.Name);
                changed = true;
            }

            if (request.Email is not null)
            {


                var newEmail = new Email(request.Email);
                if (!await _specification.IsSatisfiedByAsync(newEmail, cancellationToken))
                {
                    throw new DomainException("Email is already in use.");
                }

                if (!string.Equals(user.Email.Address, newEmail.Address, StringComparison.OrdinalIgnoreCase))
                {
                    user.ChangeEmail(newEmail, await _repository.IsEmailRegisteredAsync(newEmail.Address));
                    changed = true;
                }
            }

            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value && !user.IsActive)
                {
                    user.Activate();
                    changed = true;
                }
                else if (!request.IsActive.Value && user.IsActive)
                {
                    user.Deactivate();
                    changed = true;
                }
            }

            if (!changed)
                return ResultViewModel<UserAdminViewModel>.Error("No changes were provided.");

            await _repository.UpdateAsync(user, cancellationToken);

            return ResultViewModel<UserAdminViewModel>.Success(UserAdminViewModel.FromEntity(user));
        }
    }
}
