namespace MyApp.Worker.Models;

public sealed record CreateTeamPayload(int UniverseId, string Name, int Attackers, int Defenders);

public sealed record TeamQueueMessage
{
    public int RetryCount { get; init; }

    public required CreateTeamPayload Payload { get; init; }
}
