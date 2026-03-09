using MediatR;
using UsersAPI.Application.Models;
using UsersAPI.Domain.Interfaces;

namespace UsersAPI.Application.Commands.NewLogin;

public class NewLoginHandler(
    IUserRepository repository,
    IPasswordHasher passwordHasher,
    IAuthService authService)
    : IRequestHandler<NewLoginCommand, ResultViewModel<LoginViewModel>>
{
    public async Task<ResultViewModel<LoginViewModel>> Handle(NewLoginCommand request, CancellationToken ct)
    {
        var user = await repository.GetByEmailAsync(request.Email, ct);
        if (user is null)
            return ResultViewModel<LoginViewModel>.Error("Invalid email or password.");

        if (!passwordHasher.VerifyPassword(request.Password, user.Password.Value))
            return ResultViewModel<LoginViewModel>.Error("Invalid email or password.");

        if (!user.IsActive)
            return ResultViewModel<LoginViewModel>.Error("User is inactive.");

        var token = authService.GenerateToken(user.Id, user.Email.Address, user.Role.Value);
        return ResultViewModel<LoginViewModel>.Success(new LoginViewModel(token));
    }
}
