using MediatR;
using Users.Application.DTOs;
using Users.Domain.Interfaces;

namespace Users.Application.Queries.GetUserByEmail;

public class GetUserByEmailHandler(IUserRepository repository)
    : IRequestHandler<GetUserByEmailQuery, ResultViewModel<UserViewModel>>
{
    public async Task<ResultViewModel<UserViewModel>> Handle(GetUserByEmailQuery request, CancellationToken ct)
    {
        var user = await repository.GetByEmailAsync(request.Email, ct);
        if (user is null)
            return ResultViewModel<UserViewModel>.Error("User not found.");

        return ResultViewModel<UserViewModel>.Success(
            new UserViewModel(user.Id, user.Name.Value, user.Email.Address, user.Role.Value, user.IsActive, user.CreatedAt));
    }
}
