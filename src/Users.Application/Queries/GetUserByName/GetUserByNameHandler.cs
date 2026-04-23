using MediatR;
using Users.Application.DTOs;
using Users.Domain.Interfaces;

namespace Users.Application.Queries.GetUserByName;

public class GetUserByNameHandler(IUserRepository repository)
    : IRequestHandler<GetUserByNameQuery, ResultViewModel<List<UserViewModel>>>
{
    public async Task<ResultViewModel<List<UserViewModel>>> Handle(GetUserByNameQuery request, CancellationToken ct)
    {
        var users = await repository.GetByNameAsync(request.Name, ct);
        if (!users.Any())
            return ResultViewModel<List<UserViewModel>>.Error("No users found.");

        var vms = users.Select(u =>
            new UserViewModel(u.Id, u.Name.Value, u.Email.Address, u.Role.Value, u.IsActive, u.CreatedAt)).ToList();

        return ResultViewModel<List<UserViewModel>>.Success(vms);
    }
}
