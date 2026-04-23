using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Users.Application.Caching;
using Users.Application.Commands.ChangeUserRole;
using Users.Domain.Entities;
using Users.Domain.Exceptions;
using Users.Domain.Interfaces;
using Users.Domain.ValueObjects;

namespace Users.Tests.Application;

public class ChangeUserRoleHandlerTests
{
    private static User ExistingUser() =>
        User.Create("Test User", new Email("user@example.com"), Password.FromHash("hash"));

    [Fact]
    public async Task UserNotFound_Handle_ReturnsError()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = new ChangeUserRoleHandler(repo.Object, new Mock<IDistributedCache>().Object);
        var result = await sut.Handle(new ChangeUserRoleCommand(Guid.NewGuid(), "Admin"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found.", result.Message);
    }

    [Fact]
    public async Task InvalidRole_Handle_ThrowsDomainException()
    {
        var user = ExistingUser();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var sut = new ChangeUserRoleHandler(repo.Object, new Mock<IDistributedCache>().Object);
        var act = async () => await sut.Handle(new ChangeUserRoleCommand(user.Id, "SuperAdmin"), CancellationToken.None);

        await Assert.ThrowsAsync<DomainException>(act);
        repo.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("User")]
    public async Task ValidRole_Handle_UpdatesRoleInvalidatesCacheAndReturnsViewModel(string role)
    {
        var user  = ExistingUser();
        var cache = new Mock<IDistributedCache>();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var sut    = new ChangeUserRoleHandler(repo.Object, cache.Object);
        var result = await sut.Handle(new ChangeUserRoleCommand(user.Id, role), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(role, result.Data!.Role);
        repo.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);

        // Cache deve ser invalidado após alteração de papel
        cache.Verify(c => c.RemoveAsync(UserCacheKeys.ById(user.Id), It.IsAny<CancellationToken>()), Times.Once);
    }
}
