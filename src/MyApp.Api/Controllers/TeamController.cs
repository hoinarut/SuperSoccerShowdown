using Mediator;
using Microsoft.AspNetCore.Mvc;
using MyApp.Api.Models;
using MyApp.Application.DTOs;
using MyApp.Application.Handlers.Team;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("teams")]
public class CustomerController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TeamDto>> Create(CreateTeamRequest request, CancellationToken cancellationToken)
    {
        var team = await mediator.Send(new CreateTeamCommand(request.UniverseId, request.Name, request.Attackers, request.Defenders), cancellationToken);
        return Ok(team);
    }
}
