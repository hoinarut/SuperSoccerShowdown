using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS;
using Microsoft.Extensions.Logging;
using MyApp.Worker.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MyApp.Worker;

public class Function
{
    private readonly ITeamMessageProcessor _processor;
    private readonly ILogger<Function> _logger;

    public Function()
        : this(CreateProcessor(), CreateLogger())
    {
    }

    internal Function(ITeamMessageProcessor processor, ILogger<Function> logger)
    {
        _processor = processor;
        _logger = logger;
    }

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            _logger.LogInformation("Processing SQS message {MessageId}.", record.MessageId);
            await _processor.ProcessAsync(record.Body, CancellationToken.None);
        }
    }

    private static ITeamMessageProcessor CreateProcessor()
    {
        var queueUrl = Environment.GetEnvironmentVariable("QUEUE_URL")
            ?? throw new InvalidOperationException("QUEUE_URL environment variable is required.");

        var apiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL")
            ?? throw new InvalidOperationException("API_BASE_URL environment variable is required.");

        var maxRetries = int.TryParse(Environment.GetEnvironmentVariable("MAX_RETRIES"), out var parsedMaxRetries)
            ? parsedMaxRetries
            : 3;

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + "/"),
        };

        var teamApiClient = new TeamApiClient(httpClient, loggerFactory.CreateLogger<TeamApiClient>());

        return new TeamMessageProcessor(
            teamApiClient,
            new AmazonSQSClient(),
            queueUrl,
            maxRetries,
            loggerFactory.CreateLogger<TeamMessageProcessor>());
    }

    private static ILogger<Function> CreateLogger() =>
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Function>();
}
