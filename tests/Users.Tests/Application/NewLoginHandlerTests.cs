using Moq;
using Users.Application.Commands.NewLogin;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Interfaces;
using Users.Domain.ValueObjects;

namespace Users.Tests.Application;

public class NewLoginHandlerTests
{
    private static readonly NewLoginCommand Cmd = new("user@example.com", "Secure@123");

    private static User ActiveUser() =>
        User.Create("Test User", new Email("user@example.com"), Password.FromHash("hash"));

    [Fact]
    public async Task UserNotFound_Handle_ReturnsError()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(Cmd.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = new NewLoginHandler(repo.Object, new Mock<IPasswordHasher>().Object, new Mock<IAuthService>().Object);

        var result = await sut.Handle(Cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid email or password.", result.Message);
    }

    [Fact]
    public async Task WrongPassword_Handle_ReturnsError()
    {
        var user = ActiveUser();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(Cmd.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.VerifyPassword(Cmd.Password, user.Password.Value)).Returns(false);

        var sut = new NewLoginHandler(repo.Object, hasher.Object, new Mock<IAuthService>().Object);

        var result = await sut.Handle(Cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid email or password.", result.Message);
    }

    [Fact]
    public async Task InactiveUser_Handle_ReturnsError()
    {
        var user = ActiveUser();
        user.Deactivate();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(Cmd.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        var sut = new NewLoginHandler(repo.Object, hasher.Object, new Mock<IAuthService>().Object);

        var result = await sut.Handle(Cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("User is inactive.", result.Message);
    }

    [Fact]
    public async Task ValidCredentials_Handle_ReturnsToken()
    {
        var user = ActiveUser();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(Cmd.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.VerifyPassword(Cmd.Password, user.Password.Value)).Returns(true);

        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.GenerateToken(user.Id, user.Email.Address, user.Role.Value)).Returns("jwt-token");

        var sut = new NewLoginHandler(repo.Object, hasher.Object, auth.Object);

        var result = await sut.Handle(Cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Data!.Token);
    }
}
