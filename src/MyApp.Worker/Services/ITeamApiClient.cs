using MyApp.Worker.Models;

namespace MyApp.Worker.Services;

public interface ITeamApiClient
{
    Task<bool> CreateTeamAsync(CreateTeamPayload payload, CancellationToken cancellationToken);
}
