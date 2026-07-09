using MyApp.Domain.Entities;
using MyApp.Domain.Ports;

namespace MyApp.Infrastructure.ExternalServices;

public class PokemonUniverseDataService : IUniverseDataService
{
    public string BaseApiUrl => "https://pokeapi.co/api/v2/";

    public Task<List<int>> GetAvailableResourceIds()
    {
        throw new NotImplementedException();
    }

    public Task<Player> GeneratePlayer(int resourceId)
    {
        throw new NotImplementedException();
    }
}