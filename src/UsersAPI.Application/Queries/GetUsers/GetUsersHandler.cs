using NetDevPack.SimpleMediator;
using UsersAPI.Application.Models;
using UsersAPI.Domain.UserAggregate;

namespace UsersAPI.Application.Queries.GetUsers
{
    public class GetUsersHandler : IRequestHandler<GetUsersQuery, ResultViewModel<PagedResultViewModel<UserAdminViewModel>>>
    {
        private readonly IUserRepository _repository;

        public GetUsersHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultViewModel<PagedResultViewModel<UserAdminViewModel>>> Handle(
            GetUsersQuery request,
            CancellationToken cancellationToken)
        {
            var page = request.Page <= 0 ? 1 : request.Page;

            var pageSize = request.PageSize <= 0 ? 5 : request.PageSize;

            pageSize = Math.Min(pageSize, 100);

            var skip = (page - 1) * pageSize;

            var (items, totalCount) = await _repository.ListPagedAsync(
                request.IncludeInactive,
                skip,
                pageSize,
                cancellationToken
            );

            var users = items.Select(UserAdminViewModel.FromEntity).ToList();

            var paged = new PagedResultViewModel<UserAdminViewModel>(
                users,
                page,
                pageSize,
                totalCount
            );

            return ResultViewModel<PagedResultViewModel<UserAdminViewModel>>.Success(paged);
        }
    }
}
