using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using MyApp.Worker.Models;

namespace MyApp.Worker.Services;

public sealed class TeamMessageProcessor(
    ITeamApiClient teamApiClient,
    IAmazonSQS sqs,
    string queueUrl,
    int maxRetries,
    ILogger<TeamMessageProcessor> logger) : ITeamMessageProcessor
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task ProcessAsync(string messageBody, CancellationToken cancellationToken)
    {
        TeamQueueMessage? message;

        try
        {
            message = JsonSerializer.Deserialize<TeamQueueMessage>(messageBody, SerializerOptions);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize SQS message body: {MessageBody}", messageBody);
            return;
        }

        if (message?.Payload is null)
        {
            logger.LogError("SQS message is missing a payload: {MessageBody}", messageBody);
            return;
        }

        var succeeded = await teamApiClient.CreateTeamAsync(message.Payload, cancellationToken);
        if (succeeded)
        {
            logger.LogInformation(
                "Successfully processed team creation request for {TeamName}.",
                message.Payload.Name);
            return;
        }

        if (message.RetryCount >= maxRetries)
        {
            logger.LogError(
                "Max retries ({MaxRetries}) reached for team {TeamName}. Message will not be re-queued. Payload: {Payload}",
                maxRetries,
                message.Payload.Name,
                messageBody);
            return;
        }

        var retriedMessage = message with { RetryCount = message.RetryCount + 1 };

        await sqs.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = JsonSerializer.Serialize(retriedMessage, SerializerOptions),
        }, cancellationToken);

        logger.LogInformation(
            "Re-queued team creation request for {TeamName} with retry count {RetryCount}.",
            message.Payload.Name,
            retriedMessage.RetryCount);
    }
}
