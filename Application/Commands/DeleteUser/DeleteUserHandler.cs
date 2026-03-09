using MediatR;
using UsersAPI.Application.Models;
using UsersAPI.Domain.Interfaces;

namespace UsersAPI.Application.Commands.DeleteUser;

public class DeleteUserHandler(IUserRepository repository)
    : IRequestHandler<DeleteUserCommand, ResultViewModel<bool>>
{
    public async Task<ResultViewModel<bool>> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await repository.GetByIdAsync(request.Id, ct);
        if (user is null)
            return ResultViewModel<bool>.Error("User not found.");

        await repository.DeleteAsync(user, ct);
        return ResultViewModel<bool>.Success(true);
    }
}
