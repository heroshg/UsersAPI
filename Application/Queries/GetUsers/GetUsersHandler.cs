using MediatR;
using UsersAPI.Application.Models;
using UsersAPI.Domain.Interfaces;

namespace UsersAPI.Application.Queries.GetUsers;

public class GetUsersHandler(IUserRepository repository)
    : IRequestHandler<GetUsersQuery, ResultViewModel<PageResultViewModel<UserViewModel>>>
{
    public async Task<ResultViewModel<PageResultViewModel<UserViewModel>>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var (items, total) = await repository.ListPagedAsync(
            request.IncludeInactive,
            request.Page * request.PageSize,
            request.PageSize,
            ct);

        var vms = items.Select(u =>
            new UserViewModel(u.Id, u.Name, u.Email.Address, u.Role.Value, u.IsActive, u.CreatedAt)).ToList();

        return ResultViewModel<PageResultViewModel<UserViewModel>>.Success(
            new PageResultViewModel<UserViewModel>(vms, total, request.Page, request.PageSize));
    }
}
