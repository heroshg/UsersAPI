using FiapCloudGames.Contracts.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Users.Application.IntegrationEvents;

namespace Users.Infrastructure.Messaging;

public class SqsEventPublisher(
    ISqsPublisher sqsPublisher,
    IConfiguration configuration,
    ILogger<SqsEventPublisher> logger) : IEventPublisher
{
    public async Task PublishUserCreatedAsync(UserCreatedEvent evt, CancellationToken ct = default)
    {
        var queueUrl = configuration["AWS:SQS:UserCreatedQueueUrl"];
        if (string.IsNullOrWhiteSpace(queueUrl))
        {
            logger.LogDebug("AWS:SQS:UserCreatedQueueUrl not configured — skipping SQS publish");
            return;
        }

        await sqsPublisher.PublishAsync(evt, queueUrl, ct);
    }
}
