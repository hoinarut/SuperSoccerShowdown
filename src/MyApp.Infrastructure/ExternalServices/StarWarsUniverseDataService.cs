using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;
using MyApp.Infrastructure.ExternalServices.Models;

namespace MyApp.Infrastructure.ExternalServices;

public class StarWarsUniverseDataService(HttpClient httpClient, ILogger<StarWarsUniverseDataService> logger) : IUniverseDataService
{
    public bool CanHandle(Universe universe) =>
        universe.Name.Equals("StarWars", StringComparison.OrdinalIgnoreCase);
    public async Task<List<int>> GetAvailableResourceIds(Universe universe)
    {
        var response = await httpClient.GetFromJsonAsync<ExternalResourceCount>($"{universe.ApiUrl}people");
        return response == null ? throw new Exception("Failed to retrieve people count from Star Wars API") : Enumerable.Range(1, response.Count).ToList();
    }

    public async Task<Player?> GeneratePlayer(Universe universe, int resourceId)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<StarWarsPerson>($"{universe.ApiUrl}people/{resourceId}");
            if (response == null)
            {
                throw new Exception("Failed to retrieve person from Star Wars API");
            }

            return new Player(
                response.Name,
                ParseMeasurement(response.Mass),
                ParseMeasurement(response.Height),
                resourceId);
        }
        catch(HttpRequestException hex)
        {
            logger.LogError(hex, "Failed to generate player from Star Wars API resource {ResourceId}", resourceId);
            return null;
        }
    }

    private static double ParseMeasurement(string value)
    {
        return double.TryParse(value, out var parsed) ? parsed : 0;
    }
}