using MyApp.Domain.Entities;

namespace MyApp.Domain.Ports;

public interface IUniverseRepository : IRepository<Universe>
{
    Task<List<int>> GetUsedExternalResourceIds(int universeId);
}