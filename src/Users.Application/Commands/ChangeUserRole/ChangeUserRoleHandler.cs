using MediatR;
using Users.Application.DTOs;
using Users.Domain.Interfaces;

namespace Users.Application.Commands.ChangeUserRole;

public class ChangeUserRoleHandler(IUserRepository repository)
    : IRequestHandler<ChangeUserRoleCommand, ResultViewModel<UserViewModel>>
{
    public async Task<ResultViewModel<UserViewModel>> Handle(ChangeUserRoleCommand request, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(request.Id, ct);
        if (user is null)
            return ResultViewModel<UserViewModel>.Error("User not found.");

        user.ChangeRole(request.Role);
        await repository.UpdateAsync(user, ct);

        return ResultViewModel<UserViewModel>.Success(
            new UserViewModel(user.Id, user.Name, user.Email.Address, user.Role.Value, user.IsActive, user.CreatedAt));
    }
}
