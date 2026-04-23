using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;

namespace Users.Infrastructure.Messaging;

public class SqsPublisher(IAmazonSQS sqsClient, ILogger<SqsPublisher> logger) : ISqsPublisher
{
    public async Task PublishAsync<T>(T message, string queueUrl, CancellationToken ct = default)
        where T : class
    {
        var body = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var request = new SendMessageRequest
        {
            QueueUrl    = queueUrl,
            MessageBody = body
        };

        await sqsClient.SendMessageAsync(request, ct);

        logger.LogInformation(
            "Message published to SQS — queue={Queue} type={Type}",
            queueUrl, typeof(T).Name);
    }
}
