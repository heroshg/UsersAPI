using FiapCloudGames.Contracts.Events;

namespace Users.Application.IntegrationEvents;

/// <summary>
/// Publica eventos de integração para consumidores externos (SQS em produção, no-op em dev).
/// </summary>
public interface IEventPublisher
{
    Task PublishUserCreatedAsync(UserCreatedEvent evt, CancellationToken ct = default);
}
