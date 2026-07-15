using Mediator;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs;
using MyApp.Application.Handlers.Universe;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("universes")]
public class UniverseController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<UniverseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UniverseDto>>> List(CancellationToken cancellationToken)
    {
        var universes = await mediator.Send(new GetUniversesQuery(), cancellationToken);
        return Ok(universes);
    }
}
