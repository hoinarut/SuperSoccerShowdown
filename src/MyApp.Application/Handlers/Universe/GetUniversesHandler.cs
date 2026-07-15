using Mediator;
using MyApp.Application.DTOs;
using MyApp.Domain.Ports;

namespace MyApp.Application.Handlers.Universe;

public sealed class GetUniversesHandler(IUniverseRepository universeRepository)
    : IQueryHandler<GetUniversesQuery, List<UniverseDto>>
{
    public ValueTask<List<UniverseDto>> Handle(GetUniversesQuery query, CancellationToken cancellationToken)
    {
        var universes = universeRepository.Query.Where(x=>x.IsEnabled).ToList();

        return ValueTask.FromResult(universes
            .Select(universe => new UniverseDto(universe.Id, universe.Name, universe.ApiUrl))
            .ToList());
    }
}
