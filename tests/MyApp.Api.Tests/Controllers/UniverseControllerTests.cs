using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using MyApp.Api.Tests.Infrastructure;
using MyApp.Application.DTOs;

namespace MyApp.Api.Tests.Controllers;

public class UniverseControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _client;

    public UniverseControllerTests(CustomWebApplicationFactory factory)
    {
        factory.SeedUniverses();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_universes_ReturnsSeededUniverses()
    {
        var response = await _client.GetAsync("/universes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var universes = await response.Content.ReadFromJsonAsync<List<UniverseDto>>(JsonOptions);
        universes.Should().NotBeNull();
        universes!.Should().HaveCount(2);
        universes.Select(u => u.Name).Should().BeEquivalentTo(["Pokemon", "StarWars"]);
        universes.Should().OnlyContain(u => !string.IsNullOrWhiteSpace(u.ApiUrl));
    }
}
