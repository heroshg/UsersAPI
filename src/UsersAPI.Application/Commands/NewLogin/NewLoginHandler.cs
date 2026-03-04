using NetDevPack.SimpleMediator;
using UsersAPI.Application.Models;
using UsersAPI.Domain.UserAggregate;

namespace UsersAPI.Application.Commands.NewLogin
{
    public class NewLoginHandler : IRequestHandler<NewLoginCommand, ResultViewModel<LoginViewModel>>
    {
        private readonly IAuthService _service;
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher _passwordHasher;

        public NewLoginHandler(IAuthService service, IUserRepository repository, IPasswordHasher passwordHasher)
        {
            _service = service;
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResultViewModel<LoginViewModel>> Handle(NewLoginCommand request, CancellationToken cancellationToken)
        {

            var email = new Email(request.Email);

            var user = await _repository.GetByEmailAsync(email, cancellationToken);

            if (user is null)
            {
                return ResultViewModel<LoginViewModel>.Error("Invalid email or password");
            }

            if (!_passwordHasher.VerifyPassword(request.Password, user.Password.Value))
            {
                return ResultViewModel<LoginViewModel>.Error("Invalid email or password");
            }

            if (!user.IsActive)
            {
                return ResultViewModel<LoginViewModel>.Error("User is inactive");
            }

            var token = _service.GenerateToken(user.Id, request.Email, user.Role.Value);

            var loginViewModel = new LoginViewModel(token);

            return ResultViewModel<LoginViewModel>.Success(loginViewModel);

        }
    }
}
