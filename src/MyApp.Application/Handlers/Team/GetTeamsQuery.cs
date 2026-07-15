using Mediator;
using MyApp.Application.DTOs;

namespace MyApp.Application.Handlers.Team;

public sealed record GetTeamsQuery() : IQuery<List<TeamDto>>;
