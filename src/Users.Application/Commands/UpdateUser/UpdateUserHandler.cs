using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Users.Application.Caching;
using Users.Application.DTOs;
using Users.Domain.Interfaces;
using Users.Domain.ValueObjects;

namespace Users.Application.Commands.UpdateUser;

public class UpdateUserHandler(IUserRepository repository, IDistributedCache cache)
    : IRequestHandler<UpdateUserCommand, ResultViewModel<UserViewModel>>
{
    public async Task<ResultViewModel<UserViewModel>> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(request.Id, ct);
        if (user is null)
            return ResultViewModel<UserViewModel>.Error("User not found.");

        if (!string.IsNullOrWhiteSpace(request.Name))
            user.ChangeName(request.Name);

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            if (await repository.IsEmailRegisteredAsync(request.Email, ct))
                return ResultViewModel<UserViewModel>.Error("Email already in use.");
            user.ChangeEmail(new Email(request.Email));
        }

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value) user.Activate();
            else user.Deactivate();
        }

        await repository.UpdateAsync(user, ct);
        await cache.RemoveAsync(UserCacheKeys.ById(user.Id), ct);

        return ResultViewModel<UserViewModel>.Success(
            new UserViewModel(user.Id, user.Name.Value, user.Email.Address, user.Role.Value, user.IsActive, user.CreatedAt));
    }
}
