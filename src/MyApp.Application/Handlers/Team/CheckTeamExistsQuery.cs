using Mediator;

namespace MyApp.Application.Handlers.Team;

public sealed record CheckTeamExistsQuery(string Name) : IQuery<bool>;
