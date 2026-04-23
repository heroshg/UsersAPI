namespace Users.Application.Caching;

public static class UserCacheKeys
{
    public static string ById(Guid id) => $"users:id:{id}";

    public static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(10);
}
