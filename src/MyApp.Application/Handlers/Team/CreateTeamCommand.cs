using Mediator;
using MyApp.Application.DTOs;

namespace MyApp.Application.Handlers.Team;

public sealed record CreateTeamCommand(int UniverseId, string Name, int Attack, int Defense) : ICommand<TeamDto>;
