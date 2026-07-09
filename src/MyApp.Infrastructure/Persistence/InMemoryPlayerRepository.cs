using MyApp.Domain.Entities;
using MyApp.Domain.Enums;
using MyApp.Domain.Ports;

namespace MyApp.Infrastructure.Persistence;

public sealed class InMemoryPlayerRepository : IPlayerRepository
{
    private static readonly Player SamplePlayer = new(
        1,
        "The Player",
        85.5m, 1.75m);

    public Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(id == SamplePlayer.Id ? SamplePlayer : null);
    }
}
