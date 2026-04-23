using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Users.Application.Caching;
using Users.Application.Commands.UpdateUser;
using Users.Domain.Entities;
using Users.Domain.Interfaces;
using Users.Domain.ValueObjects;

namespace Users.Tests.Application;

public class UpdateUserHandlerTests
{
    private static User ExistingUser() =>
        User.Create("Old Name", new Email("old@example.com"), Password.FromHash("hash"));

    [Fact]
    public async Task UserNotFound_Handle_ReturnsError()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = new UpdateUserHandler(repo.Object, new Mock<IDistributedCache>().Object);
        var result = await sut.Handle(new UpdateUserCommand(Guid.NewGuid(), null, null, null), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found.", result.Message);
    }

    [Fact]
    public async Task EmailAlreadyInUse_Handle_ReturnsError()
    {
        var user = ExistingUser();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        repo.Setup(r => r.IsEmailRegisteredAsync("taken@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new UpdateUserHandler(repo.Object, new Mock<IDistributedCache>().Object);
        var result = await sut.Handle(new UpdateUserCommand(user.Id, null, "taken@example.com", null), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Email already in use.", result.Message);
        repo.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ValidNameChange_Handle_UpdatesNameInvalidatesCacheAndPersists()
    {
        var user  = ExistingUser();
        var cache = new Mock<IDistributedCache>();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var sut    = new UpdateUserHandler(repo.Object, cache.Object);
        var result = await sut.Handle(new UpdateUserCommand(user.Id, "New Name", null, null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Data!.Name);
        repo.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        cache.Verify(c => c.RemoveAsync(UserCacheKeys.ById(user.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidEmailChange_Handle_UpdatesEmailInvalidatesCacheAndPersists()
    {
        var user  = ExistingUser();
        var cache = new Mock<IDistributedCache>();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        repo.Setup(r => r.IsEmailRegisteredAsync("new@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut    = new UpdateUserHandler(repo.Object, cache.Object);
        var result = await sut.Handle(new UpdateUserCommand(user.Id, null, "new@example.com", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("new@example.com", result.Data!.Email);
        cache.Verify(c => c.RemoveAsync(UserCacheKeys.ById(user.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task IsActiveFlag_Handle_UpdatesActivationAndInvalidatesCache(bool isActive)
    {
        var user  = ExistingUser();
        var cache = new Mock<IDistributedCache>();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var sut = new UpdateUserHandler(repo.Object, cache.Object);
        await sut.Handle(new UpdateUserCommand(user.Id, null, null, isActive), CancellationToken.None);

        Assert.Equal(isActive, user.IsActive);
        cache.Verify(c => c.RemoveAsync(UserCacheKeys.ById(user.Id), It.IsAny<CancellationToken>()), Times.Once);
    }
}
