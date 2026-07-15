using Mediator;
using MyApp.Application.DTOs;
using MyApp.Domain.Ports;

namespace MyApp.Application.Handlers.Team;

public sealed class GetTeamsHandler(ITeamRepository teamRepository)
    : IQueryHandler<GetTeamsQuery, List<TeamDto>>
{
    public async ValueTask<List<TeamDto>> Handle(GetTeamsQuery query, CancellationToken cancellationToken)
    {
        var teams = await teamRepository.GetAllAsync(cancellationToken);

        return (teams.Count == 0
            ? []
            : teams.Select(t=> new TeamDto(t.Id, t.Name, t.AttackersCount, t.DefendersCount, 
                t.Players?.Select(p => new PlayerDto(p.Id, p.Name, p.Weight, p.Height, p.Type, p.ExternalResourceId)).ToList()!)).ToList());
    }
}
