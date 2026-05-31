using FiapCloudGames.Contracts.Events;

namespace Users.Application.IntegrationEvents;

public interface IEventPublisher
{
    Task PublishUserCreatedAsync(UserCreatedEvent evt, CancellationToken ct = default);
}
