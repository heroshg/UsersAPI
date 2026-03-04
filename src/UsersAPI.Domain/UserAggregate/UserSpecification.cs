namespace UsersAPI.Domain.UserAggregate
{
    public class UserSpecification
    {
        private readonly IUserRepository _repository;

        public UserSpecification(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> IsSatisfiedByAsync(Email email, CancellationToken ct)
        {
            return !await _repository.IsEmailRegisteredAsync(email.Address);
        }
    }
}
