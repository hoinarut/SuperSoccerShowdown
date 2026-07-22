using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.ExternalServices;

namespace MyApp.Infrastructure.Tests.ExternalServices;

public class StarWarsUniverseDataServiceTests
{
    private static readonly Universe Universe = new()
    {
        Id = 1,
        Name = "StarWars",
        ApiUrl = "https://swapi.dev/api/"
    };

    [Fact]
    public async Task GetAvailableResourceIds_CallsPeopleEndpointAndReturnsRange()
    {
        var handler = new MockHttpMessageHandler(_ =>
            MockHttpMessageHandler.JsonResponse("""{"count": 3}"""));
        var service = CreateService(handler);

        var resourceIds = await service.GetAvailableResourceIds(Universe);

        resourceIds.Should().Equal(1, 2, 3);
        handler.Requests.Should().ContainSingle()
            .Which.RequestUri!.ToString().Should().Be("https://swapi.dev/api/people");
    }

    [Fact]
    public async Task GetAvailableResourceIds_WhenResponseIsNull_ThrowsException()
    {
        var handler = new MockHttpMessageHandler(_ =>
            MockHttpMessageHandler.JsonResponse("null"));
        var service = CreateService(handler);

        var action = () => service.GetAvailableResourceIds(Universe);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Failed to retrieve people count from Star Wars API");
    }

    [Fact]
    public async Task GeneratePlayer_MapsPersonToPlayer()
    {
        var handler = new MockHttpMessageHandler(_ =>
            MockHttpMessageHandler.JsonResponse("""
                {
                  "name": "Luke Skywalker",
                  "mass": "77",
                  "height": "172"
                }
                """));
        var service = CreateService(handler);

        var player = await service.GeneratePlayer(Universe, resourceId: 1);

        player.Should().NotBeNull();
        player!.Name.Should().Be("Luke Skywalker");
        player.Weight.Value.Should().Be(77);
        player.Height.Value.Should().Be(172);
        player.ExternalResourceId.Should().Be(1);
        handler.Requests.Should().ContainSingle()
            .Which.RequestUri!.ToString().Should().Be("https://swapi.dev/api/people/1");
    }

    [Fact]
    public async Task GeneratePlayer_WhenMassIsUnknown_ParsesMeasurementAsZero()
    {
        var handler = new MockHttpMessageHandler(_ =>
            MockHttpMessageHandler.JsonResponse("""
                {
                  "name": "Chewbacca",
                  "mass": "unknown",
                  "height": "200"
                }
                """));
        var service = CreateService(handler);

        var player = await service.GeneratePlayer(Universe, resourceId: 13);

        player.Should().NotBeNull();
        player!.Weight.Value.Should().Be(0);
        player.Height.Value.Should().Be(200);
        player.HasValidMeasurements.Should().BeFalse();
    }

    [Fact]
    public async Task GeneratePlayer_WhenHttpRequestFails_ReturnsNullAndLogsError()
    {
        var logger = new Mock<ILogger<StarWarsUniverseDataService>>();
        var handler = new MockHttpMessageHandler(_ => throw new HttpRequestException("Network error"));
        var service = CreateService(handler, logger.Object);

        var player = await service.GeneratePlayer(Universe, resourceId: 99);

        player.Should().BeNull();
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Failed to generate player from Star Wars API resource 99")),
                It.IsAny<HttpRequestException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("StarWars", true)]
    [InlineData("starwars", true)]
    [InlineData("Pokemon", false)]
    public void CanHandle_MatchesStarWarsUniverseOnly(string universeName, bool expected)
    {
        var service = CreateService(new MockHttpMessageHandler(_ => throw new InvalidOperationException()));
        var universe = new Universe { Name = universeName, ApiUrl = "https://swapi.dev/api/" };

        service.CanHandle(universe).Should().Be(expected);
    }
    
    private static StarWarsUniverseDataService CreateService(
        MockHttpMessageHandler handler,
        ILogger<StarWarsUniverseDataService>? logger = null) =>
        new(HttpClientTestHelper.CreateClient(handler), logger ?? Mock.Of<ILogger<StarWarsUniverseDataService>>());
}
