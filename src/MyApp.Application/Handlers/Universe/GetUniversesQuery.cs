using Mediator;
using MyApp.Application.DTOs;

namespace MyApp.Application.Handlers.Universe;

public sealed record GetUniversesQuery() : IQuery<List<UniverseDto>>;
