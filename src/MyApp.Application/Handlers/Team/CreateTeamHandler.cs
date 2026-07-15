using Mediator;
using MyApp.Application.DTOs;
using MyApp.Domain.Ports;

namespace MyApp.Application.Handlers.Team;

public sealed class CreateTeamHandler(ITeamManager teamManager, IUniverseRepository universeRepository)
    : ICommandHandler<CreateTeamCommand, TeamDto?>
{
    public async ValueTask<TeamDto?> Handle(CreateTeamCommand command, CancellationToken cancellationToken)
    {
        var team = await teamManager.CreateTeam(command.UniverseId, command.Name, command.Attack, command.Defense);

        if (team == null)
        {
            return null;
        }

        var universeName = universeRepository.Query
            .FirstOrDefault(universe => universe.Id == team.UniverseId)?.Name ?? "Unknown";

        return new TeamDto(
            team.Id,
            team.Name,
            team.UniverseId,
            universeName,
            team.AttackersCount,
            team.DefendersCount,
            team.Players!.Select(player =>
                new PlayerDto(player.Id, player.Name, player.Weight, player.Height, player.Type, player.ExternalResourceId)).ToList());
    }
}
