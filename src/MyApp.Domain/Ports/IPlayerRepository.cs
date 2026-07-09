using MyApp.Domain.Entities;

namespace MyApp.Domain.Ports;

public interface IPlayerRepository
{
    Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
