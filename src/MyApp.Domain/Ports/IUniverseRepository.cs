using MyApp.Domain.Entities;

namespace MyApp.Domain.Ports;

public interface IUniverseRepository
{
    Task<Universe?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
