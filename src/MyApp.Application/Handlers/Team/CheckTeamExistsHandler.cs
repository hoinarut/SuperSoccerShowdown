using Mediator;
using MyApp.Domain.Ports;

namespace MyApp.Application.Handlers.Team;

public sealed class CheckTeamExistsHandler(ITeamRepository teamRepository)
    : IQueryHandler<CheckTeamExistsQuery, bool>
{
    public ValueTask<bool> Handle(CheckTeamExistsQuery query, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(teamRepository.Query.Any(t => t.Name == query.Name));
    }
}
