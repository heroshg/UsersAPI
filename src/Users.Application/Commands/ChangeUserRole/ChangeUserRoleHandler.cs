using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Users.Application.Caching;
using Users.Application.DTOs;
using Users.Domain.Interfaces;

namespace Users.Application.Commands.ChangeUserRole;

public class ChangeUserRoleHandler(IUserRepository repository, IDistributedCache cache)
    : IRequestHandler<ChangeUserRoleCommand, ResultViewModel<UserViewModel>>
{
    public async Task<ResultViewModel<UserViewModel>> Handle(ChangeUserRoleCommand request, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(request.Id, ct);
        if (user is null)
            return ResultViewModel<UserViewModel>.Error("User not found.");

        user.ChangeRole(request.Role);
        await repository.UpdateAsync(user, ct);
        await cache.RemoveAsync(UserCacheKeys.ById(user.Id), ct);

        return ResultViewModel<UserViewModel>.Success(
            new UserViewModel(user.Id, user.Name.Value, user.Email.Address, user.Role.Value, user.IsActive, user.CreatedAt));
    }
}
