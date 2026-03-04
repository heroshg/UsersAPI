using NetDevPack.SimpleMediator;
using UsersAPI.Application.Models;
using UsersAPI.Domain.UserAggregate;

namespace UsersAPI.Application.Commands.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, ResultViewModel<Guid>>
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserSpecification _specification;

        public RegisterUserHandler(IUserRepository repository, IPasswordHasher passwordHasher, IUserSpecification specification)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _specification = specification;
        }

        public async Task<ResultViewModel<Guid>> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            var email = new Email(request.Email);
            if (!await _specification.IsSatisfiedByAsync(email, cancellationToken))
            {
                return ResultViewModel<Guid>.Error("Email already in use.");
            }
            var plainPassword = Password.FromPlainText(request.Password);

            var passwordHash = _passwordHasher.HashPassword(plainPassword.Value);

            var user = User.Create(
                request.Name,
                email,
                Password.FromHash(passwordHash)
            );

            var id = await _repository.AddAsync(user, cancellationToken);

            return ResultViewModel<Guid>.Success(id);
        }
    }
}
