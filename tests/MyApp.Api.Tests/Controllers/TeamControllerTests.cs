using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using MyApp.Api.Tests.Infrastructure;
using MyApp.Application.DTOs;
using MyApp.Domain;
using MyApp.Domain.Enums;

namespace MyApp.Api.Tests.Controllers;

public class TeamControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TeamControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        factory.SeedUniverses();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_teams_ValidRequest_Returns200WithTeam()
    {
        var universeId = _factory.GetStarWarsUniverseId();
        var request = new
        {
            universeId,
            name = "Dream Team",
            attackers = 1,
            defenders = 3
        };

        var response = await _client.PostAsJsonAsync("/teams", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var team = await response.Content.ReadFromJsonAsync<TeamDto>(JsonOptions);
        team.Should().NotBeNull();
        team!.Name.Should().Be("Dream Team");
        team.UniverseName.Should().Be("StarWars");
        team.Attackers.Should().Be(1);
        team.Defenders.Should().Be(3);
        team.Players.Should().HaveCount(Constants.NumberOfPlayers);
        team.Players.Should().ContainSingle(p => p.PlayerType == PlayerType.Goalie);
        team.Players.Where(p => p.PlayerType == PlayerType.Defence).Should().HaveCount(3);
        team.Players.Should().ContainSingle(p => p.PlayerType == PlayerType.Offence);
    }

    [Fact]
    public async Task POST_teams_DuplicateName_Returns400()
    {
        var universeId = _factory.GetStarWarsUniverseId();
        var request = new
        {
            universeId,
            name = "Duplicate Team",
            attackers = 1,
            defenders = 3
        };

        var firstResponse = await _client.PostAsJsonAsync("/teams", request);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var duplicateResponse = await _client.PostAsJsonAsync("/teams", request);

        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await duplicateResponse.Content.ReadAsStringAsync();
        body.Should().Contain("Team with the same name already exists.");
    }

    [Fact]
    public async Task POST_teams_InvalidLineup_Returns400()
    {
        var universeId = _factory.GetStarWarsUniverseId();
        var request = new
        {
            universeId,
            name = "Invalid Lineup",
            attackers = 3,
            defenders = 3
        };

        var response = await _client.PostAsJsonAsync("/teams", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_teams_ReturnsPersistedTeams()
    {
        var universeId = _factory.GetStarWarsUniverseId();
        var createRequest = new
        {
            universeId,
            name = "Listed Team",
            attackers = 2,
            defenders = 2
        };

        var createResponse = await _client.PostAsJsonAsync("/teams", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var listResponse = await _client.GetAsync("/teams");

        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var teams = await listResponse.Content.ReadFromJsonAsync<List<TeamDto>>(JsonOptions);
        teams.Should().NotBeNull();
        teams!.Should().ContainSingle(t => t.Name == "Listed Team");
        teams.Single(t => t.Name == "Listed Team").Players.Should().HaveCount(Constants.NumberOfPlayers);
    }
}
