using MyApp.Domain.Entities;

namespace MyApp.Domain.Ports;

public interface ITeamRepository
{
    Task<Team> CreateAsync(Team team, CancellationToken cancellationToken = default);
}
