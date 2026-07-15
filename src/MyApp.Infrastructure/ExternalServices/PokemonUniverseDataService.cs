using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;
using MyApp.Infrastructure.ExternalServices.Models;

namespace MyApp.Infrastructure.ExternalServices;

public class PokemonUniverseDataService(HttpClient httpClient, ILogger<PokemonUniverseDataService> logger) : IUniverseDataService
{
    public bool CanHandle(Universe universe) =>
        universe.Name.Equals("Pokemon", StringComparison.OrdinalIgnoreCase);
    
    public async Task<List<int>> GetAvailableResourceIds(Universe universe)
    {
        var response = await httpClient.GetFromJsonAsync<ExternalResourceCount>($"{universe.ApiUrl}pokemon");
        return response == null ? throw new Exception("Failed to retrieve people count from Pokemon API") : Enumerable.Range(1, response.Count).ToList();
    }

    public async Task<Player?> GeneratePlayer(Universe universe, int resourceId)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<PokemonPerson>($"{universe.ApiUrl}pokemon/{resourceId}");
            if (response == null)
            {
                throw new Exception("Failed to retrieve person from Pokemon API");
            }

            return new Player(
                response.Name,
                response.Weight / 10,
                response.Height * 10,
                resourceId);
        }
        catch(HttpRequestException hex)
        {
            logger.LogError(hex, "Failed to generate player from Pokemon API resource {ResourceId}", resourceId);
            return null;
        }
    }
}