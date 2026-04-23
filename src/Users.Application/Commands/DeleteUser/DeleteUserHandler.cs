using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Users.Application.Caching;
using Users.Application.DTOs;
using Users.Domain.Interfaces;

namespace Users.Application.Commands.DeleteUser;

public class DeleteUserHandler(IUserRepository repository, IDistributedCache cache)
    : IRequestHandler<DeleteUserCommand, ResultViewModel<bool>>
{
    public async Task<ResultViewModel<bool>> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(request.Id, ct);
        if (user is null)
            return ResultViewModel<bool>.Error("User not found.");

        await repository.DeleteAsync(user, ct);
        await cache.RemoveAsync(UserCacheKeys.ById(user.Id), ct);

        return ResultViewModel<bool>.Success(true);
    }
}
