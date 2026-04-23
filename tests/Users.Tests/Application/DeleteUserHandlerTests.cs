using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Users.Application.Caching;
using Users.Application.Commands.DeleteUser;
using Users.Domain.Entities;
using Users.Domain.Interfaces;
using Users.Domain.ValueObjects;

namespace Users.Tests.Application;

public class DeleteUserHandlerTests
{
    private static User ExistingUser() =>
        User.Create("Test User", new Email("user@example.com"), Password.FromHash("hash"));

    [Fact]
    public async Task UserNotFound_Handle_ReturnsError()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = new DeleteUserHandler(repo.Object, new Mock<IDistributedCache>().Object);
        var result = await sut.Handle(new DeleteUserCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found.", result.Message);
        repo.Verify(r => r.DeleteAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExistingUser_Handle_DeletesInvalidatesCacheAndReturnsSuccess()
    {
        var user  = ExistingUser();
        var cache = new Mock<IDistributedCache>();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var sut    = new DeleteUserHandler(repo.Object, cache.Object);
        var result = await sut.Handle(new DeleteUserCommand(user.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
        repo.Verify(r => r.DeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once);

        // Cache deve ser invalidado após remoção do usuário
        cache.Verify(c => c.RemoveAsync(UserCacheKeys.ById(user.Id), It.IsAny<CancellationToken>()), Times.Once);
    }
}
