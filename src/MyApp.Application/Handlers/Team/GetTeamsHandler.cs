using Mediator;
using MyApp.Application.DTOs;
using MyApp.Domain.Ports;

namespace MyApp.Application.Handlers.Team;

public sealed class GetTeamsHandler(ITeamRepository teamRepository, IUniverseRepository universeRepository)
    : IQueryHandler<GetTeamsQuery, List<TeamDto>>
{
    public async ValueTask<List<TeamDto>> Handle(GetTeamsQuery query, CancellationToken cancellationToken)
    {
        var teams = await teamRepository.GetAllAsync(cancellationToken);

        if (teams.Count == 0)
        {
            return [];
        }

        var universeNames = universeRepository.Query.ToDictionary(universe => universe.Id, universe => universe.Name);

        return teams.Select(team => new TeamDto(
            team.Id,
            team.Name,
            team.UniverseId,
            universeNames.GetValueOrDefault(team.UniverseId, "Unknown"),
            team.AttackersCount,
            team.DefendersCount,
            team.Players?.Select(player =>
                new PlayerDto(player.Id, player.Name, player.Weight, player.Height, player.Type, player.ExternalResourceId)).ToList()!)).ToList();
    }
}
