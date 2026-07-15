using System.Net;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using MyApp.Api.Models;
using MyApp.Application.DTOs;
using MyApp.Application.Handlers.Team;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("teams")]
public class TeamController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TeamDto>> Create(CreateTeamRequest request, CancellationToken cancellationToken)
    {
        var teamExists = await mediator.Send(new CheckTeamExistsQuery(request.Name), cancellationToken);
        if (teamExists)
        {
            return BadRequest("Team with the same name already exists.");
        }

        var team = await mediator.Send(new CreateTeamCommand(request.UniverseId, request.Name, request.Attackers, request.Defenders), cancellationToken);
        if(team != null)
        {
            return Ok(team);
        }
        return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to create team.");
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(List<TeamDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TeamDto>>> List(CancellationToken cancellationToken)
    {
        var teams = await mediator.Send(new GetTeamsQuery(), cancellationToken);
        return Ok(teams);
    }
}
