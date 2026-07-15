using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.ExternalServices;

namespace MyApp.Infrastructure.Tests.ExternalServices;

public class PokemonUniverseDataServiceTests
{
    private static readonly Universe Universe = new()
    {
        Id = 1,
        Name = "Pokemon",
        ApiUrl = "https://pokeapi.co/api/v2/"
    };

    [Fact]
    public async Task GetAvailableResourceIds_CallsPokemonEndpointAndReturnsRange()
    {
        var handler = new MockHttpMessageHandler(_ =>
            MockHttpMessageHandler.JsonResponse("""{"count": 4}"""));
        var service = CreateService(handler);

        var resourceIds = await service.GetAvailableResourceIds(Universe);

        resourceIds.Should().Equal(1, 2, 3, 4);
        handler.Requests.Should().ContainSingle()
            .Which.RequestUri!.ToString().Should().Be("https://pokeapi.co/api/v2/pokemon");
    }

    [Fact]
    public async Task GetAvailableResourceIds_WhenResponseIsNull_ThrowsException()
    {
        var handler = new MockHttpMessageHandler(_ =>
            MockHttpMessageHandler.JsonResponse("null"));
        var service = CreateService(handler);

        var action = () => service.GetAvailableResourceIds(Universe);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Failed to retrieve people count from Pokemon API");
    }

    [Fact]
    public async Task GeneratePlayer_MapsPokemonToPlayer()
    {
        var handler = new MockHttpMessageHandler(_ =>
            MockHttpMessageHandler.JsonResponse("""
                {
                  "name": "pikachu",
                  "weight": 60,
                  "height": 4
                }
                """));
        var service = CreateService(handler);

        var player = await service.GeneratePlayer(Universe, resourceId: 25);

        player.Should().NotBeNull();
        player!.Name.Should().Be("pikachu");
        player.Weight.Should().Be(6);
        player.Height.Should().Be(40);
        player.ExternalResourceId.Should().Be(25);
        handler.Requests.Should().ContainSingle()
            .Which.RequestUri!.ToString().Should().Be("https://pokeapi.co/api/v2/pokemon/25");
    }

    [Fact]
    public async Task GeneratePlayer_WhenHttpRequestFails_ReturnsNullAndLogsError()
    {
        var logger = new Mock<ILogger<PokemonUniverseDataService>>();
        var handler = new MockHttpMessageHandler(_ => throw new HttpRequestException("Network error"));
        var service = CreateService(handler, logger.Object);

        var player = await service.GeneratePlayer(Universe, resourceId: 99);

        player.Should().BeNull();
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Failed to generate player from Pokemon API resource 99")),
                It.IsAny<HttpRequestException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("Pokemon", true)]
    [InlineData("pokemon", true)]
    [InlineData("StarWars", false)]
    public void CanHandle_MatchesStarWarsUniverseOnly(string universeName, bool expected)
    {
        var service = CreateService(new MockHttpMessageHandler(_ => throw new InvalidOperationException()));
        var universe = new Universe { Name = universeName, ApiUrl = "https://pokeapi.co/" };

        service.CanHandle(universe).Should().Be(expected);
    }
    
    private static PokemonUniverseDataService CreateService(
        MockHttpMessageHandler handler,
        ILogger<PokemonUniverseDataService>? logger = null) =>
        new(HttpClientTestHelper.CreateClient(handler), logger ?? Mock.Of<ILogger<PokemonUniverseDataService>>());
}
