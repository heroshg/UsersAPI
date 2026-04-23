using FiapCloudGames.Contracts.Events;
using MassTransit;
using Moq;
using Users.Application.Commands.RegisterUser;
using Users.Application.IntegrationEvents;
using Users.Domain.Entities;
using Users.Domain.Interfaces;
using Users.Domain.ValueObjects;

namespace Users.Tests.Application;

public class RegisterUserHandlerTests
{
    private static RegisterUserCommand ValidCommand => new("Test User", "user@example.com", "Secure@123");

    [Fact]
    public async Task EmailAlreadyRegistered_Handle_ReturnsError()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.IsEmailRegisteredAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new RegisterUserHandler(
            repo.Object,
            new Mock<IPasswordHasher>().Object,
            new Mock<IPublishEndpoint>().Object,
            new Mock<IEventPublisher>().Object);

        var result = await sut.Handle(ValidCommand, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Email already in use.", result.Message);
        repo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Valid_Handle_CreatesUserPublishesBothEventsReturnsId()
    {
        var expectedId = Guid.NewGuid();

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.IsEmailRegisteredAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");

        var rabbitPublisher = new Mock<IPublishEndpoint>();
        var sqsPublisher    = new Mock<IEventPublisher>();

        var sut = new RegisterUserHandler(repo.Object, hasher.Object, rabbitPublisher.Object, sqsPublisher.Object);

        var result = await sut.Handle(ValidCommand, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedId, result.Data);

        repo.Verify(r => r.AddAsync(
            It.Is<User>(u =>
                u.Email.Address == "user@example.com" &&
                u.Name.Value == "Test User" &&
                u.Role.Value == Role.User.Value),
            It.IsAny<CancellationToken>()), Times.Once);

        // RabbitMQ (dev local / K8s)
        rabbitPublisher.Verify(p => p.Publish(
            It.Is<UserCreatedEvent>(e => e.UserId == expectedId && e.Email == "user@example.com"),
            It.IsAny<CancellationToken>()), Times.Once);

        // SQS (produção AWS → Lambda de notificações)
        sqsPublisher.Verify(p => p.PublishUserCreatedAsync(
            It.Is<UserCreatedEvent>(e => e.UserId == expectedId && e.Email == "user@example.com"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WeakPassword_Handle_ThrowsDomainException()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.IsEmailRegisteredAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = new RegisterUserHandler(
            repo.Object,
            new Mock<IPasswordHasher>().Object,
            new Mock<IPublishEndpoint>().Object,
            new Mock<IEventPublisher>().Object);

        var cmd = ValidCommand with { Password = "weak" };
        var act = async () => await sut.Handle(cmd, CancellationToken.None);

        await Assert.ThrowsAsync<Users.Domain.Exceptions.DomainException>(act);
        repo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
