namespace Users.Infrastructure.Messaging;

public interface ISqsPublisher
{
    Task PublishAsync<T>(T message, string queueUrl, CancellationToken ct = default) where T : class;
}
