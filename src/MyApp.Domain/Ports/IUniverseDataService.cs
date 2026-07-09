using MyApp.Domain.Entities;

namespace MyApp.Domain.Ports;

public interface IUniverseDataService
{
    string BaseApiUrl { get; }
    
    Task<List<int>> GetAvailableResourceIds();
    Task<Player> GeneratePlayer(int resourceId);
}