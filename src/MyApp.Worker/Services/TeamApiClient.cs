using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using MyApp.Worker.Models;

namespace MyApp.Worker.Services;

public sealed class TeamApiClient(HttpClient httpClient, ILogger<TeamApiClient> logger) : ITeamApiClient
{
    public async Task<bool> CreateTeamAsync(CreateTeamPayload payload, CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("teams", payload, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning(
                "Team creation API request failed with status {StatusCode}: {ResponseBody}",
                (int)response.StatusCode,
                body);

            return false;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            logger.LogWarning(ex, "Team creation API request failed due to a network error.");
            return false;
        }
    }
}
