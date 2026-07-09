using System.Net.Http.Json;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;
using MyApp.Infrastructure.ExternalServices.Models;

namespace MyApp.Infrastructure.ExternalServices;

public class StarWarsUniverseDataService(HttpClient httpClient) : IUniverseDataService
{
    public string BaseApiUrl => "https://swapi.dev/api/";

    public async Task<List<int>> GetAvailableResourceIds()
    {
        var response = await httpClient.GetFromJsonAsync<StarWarsPeopleCount>($"{BaseApiUrl}people");
        return response == null ? throw new Exception("Failed to retrieve people count from Star Wars API") : Enumerable.Range(1, response.Count).ToList();
    }

    public async Task<Player> GeneratePlayer(int resourceId)
    {
        var response = await httpClient.GetFromJsonAsync<StarWarsPerson>($"{BaseApiUrl}people/{resourceId}");
        return response == null ? throw new Exception("Failed to retrieve person from Star Wars API") : 
            new Player(response.Name, response.Mass, response.Height);
    }
}