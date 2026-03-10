namespace Users.Application.DTOs;

public record UserViewModel(Guid Id, string Name, string Email, string Role, bool IsActive, DateTime CreatedAt);

public record PageResultViewModel<T>(List<T> Items, int Total, int Page, int PageSize);
