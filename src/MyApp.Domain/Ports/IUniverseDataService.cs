using MyApp.Domain.Entities;

namespace MyApp.Domain.Ports;

public interface IUniverseDataService
{
    bool CanHandle(Universe universe);
    Task<List<int>> GetAvailableResourceIds(Universe universe);
    Task<Player?> GeneratePlayer(Universe universe, int resourceId);
}