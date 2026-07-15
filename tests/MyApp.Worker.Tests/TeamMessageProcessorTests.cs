using Amazon.SQS.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Testing.Common;
using MyApp.Worker.Models;
using MyApp.Worker.Services;

namespace MyApp.Worker.Tests;

public sealed class TeamMessageProcessorTests : BaseUnitTests
{
    private const string QueueUrl = "https://example.com/queue";

    private readonly Mock<ITeamApiClient> _teamApiClientMock = new(MockBehavior.Strict);
    private readonly Mock<Amazon.SQS.IAmazonSQS> _sqsMock = new(MockBehavior.Strict);
    private readonly Mock<ILogger<TeamMessageProcessor>> _loggerMock = new();

    protected override void VerifyAllMocks()
    {
        _teamApiClientMock.VerifyAll();
        _sqsMock.VerifyAll();
    }

    [Fact]
    public async Task ProcessAsync_WhenApiSucceeds_DoesNotRequeueMessage()
    {
        var payload = new CreateTeamPayload(1, "Dream Team", 1, 3);
        var messageBody = """{"retryCount":0,"payload":{"universeId":1,"name":"Dream Team","attackers":1,"defenders":3}}""";

        _teamApiClientMock
            .Setup(client => client.CreateTeamAsync(payload, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var processor = CreateProcessor(maxRetries: 3);

        await processor.ProcessAsync(messageBody, CancellationToken.None);

        VerifyAllMocks();
    }

    [Fact]
    public async Task ProcessAsync_WhenApiFailsAndRetriesRemain_RequeuesWithIncrementedRetryCount()
    {
        var payload = new CreateTeamPayload(1, "Dream Team", 1, 3);
        var messageBody = """{"retryCount":1,"payload":{"universeId":1,"name":"Dream Team","attackers":1,"defenders":3}}""";

        _teamApiClientMock
            .Setup(client => client.CreateTeamAsync(payload, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _sqsMock
            .Setup(sqs => sqs.SendMessageAsync(It.Is<SendMessageRequest>(request =>
                    request.QueueUrl == QueueUrl &&
                    request.MessageBody.Contains("\"retryCount\":2")),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendMessageResponse());

        var processor = CreateProcessor(maxRetries: 3);

        await processor.ProcessAsync(messageBody, CancellationToken.None);

        VerifyAllMocks();
    }

    [Fact]
    public async Task ProcessAsync_WhenApiFailsAndMaxRetriesReached_DoesNotRequeueMessage()
    {
        var payload = new CreateTeamPayload(1, "Dream Team", 1, 3);
        var messageBody = """{"retryCount":3,"payload":{"universeId":1,"name":"Dream Team","attackers":1,"defenders":3}}""";

        _teamApiClientMock
            .Setup(client => client.CreateTeamAsync(payload, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        SetupLoggerMock(_loggerMock, LogLevel.Error);

        var processor = CreateProcessor(maxRetries: 3);

        await processor.ProcessAsync(messageBody, CancellationToken.None);

        VerifyLog(_loggerMock, LogLevel.Error, Times.Once(), "Max retries (3) reached");
        VerifyAllMocks();
    }

    [Fact]
    public async Task ProcessAsync_WhenMessageIsInvalid_DoesNotCallApiOrRequeue()
    {
        var processor = CreateProcessor(maxRetries: 3);

        await processor.ProcessAsync("not-json", CancellationToken.None);

        _teamApiClientMock.Verify(
            client => client.CreateTeamAsync(It.IsAny<CreateTeamPayload>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _sqsMock.Verify(
            sqs => sqs.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private TeamMessageProcessor CreateProcessor(int maxRetries) =>
        new(
            _teamApiClientMock.Object,
            _sqsMock.Object,
            QueueUrl,
            maxRetries,
            _loggerMock.Object);
}
