using MediatR;
using Users.Application.DTOs;
using Users.Domain.Interfaces;

namespace Users.Application.Queries.GetUserById;

public class GetUserByIdHandler(IUserRepository repository)
    : IRequestHandler<GetUserByIdQuery, ResultViewModel<UserViewModel>>
{
    public async Task<ResultViewModel<UserViewModel>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(request.Id, ct);
        if (user is null)
            return ResultViewModel<UserViewModel>.Error("User not found.");

        return ResultViewModel<UserViewModel>.Success(
            new UserViewModel(user.Id, user.Name, user.Email.Address, user.Role.Value, user.IsActive, user.CreatedAt));
    }
}
