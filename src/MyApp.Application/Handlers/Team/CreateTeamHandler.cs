using Mediator;
using MyApp.Application.DTOs;
using MyApp.Domain.Ports;

namespace MyApp.Application.Handlers.Team;

public sealed class CreateTeamHandler(ITeamManager teamManager)
    : ICommandHandler<CreateTeamCommand, TeamDto?>
{
    public async ValueTask<TeamDto?> Handle(CreateTeamCommand command, CancellationToken cancellationToken)
    {
        var team = await teamManager.CreateTeam(command.UniverseId, command.Name, command.Attack, command.Defense);

        return team != null
            ? new TeamDto(team.Id, team.Name, team.AttackersCount, team.DefendersCount,
                team.Players!.Select(p =>
                    new PlayerDto(p.Id, p.Name, p.Weight, p.Height, p.Type, p.ExternalResourceId)).ToList())
            : null;
    }
}
