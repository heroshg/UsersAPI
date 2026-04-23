using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Users.Application.Caching;
using Users.Application.DTOs;
using Users.Domain.Interfaces;

namespace Users.Application.Queries.GetUserById;

public class GetUserByIdHandler(IUserRepository repository, IDistributedCache cache)
    : IRequestHandler<GetUserByIdQuery, ResultViewModel<UserViewModel>>
{
    public async Task<ResultViewModel<UserViewModel>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var cacheKey = UserCacheKeys.ById(request.Id);

        var cached = await cache.GetStringAsync(cacheKey, ct);
        if (cached is not null)
        {
            var vm = JsonSerializer.Deserialize<UserViewModel>(cached)!;
            return ResultViewModel<UserViewModel>.Success(vm);
        }

        var user = await repository.GetByIdAsync(request.Id, ct);
        if (user is null)
            return ResultViewModel<UserViewModel>.Error("User not found.");

        var viewModel = new UserViewModel(user.Id, user.Name.Value, user.Email.Address, user.Role.Value, user.IsActive, user.CreatedAt);

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(viewModel),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = UserCacheKeys.DefaultTtl }, ct);

        return ResultViewModel<UserViewModel>.Success(viewModel);
    }
}
