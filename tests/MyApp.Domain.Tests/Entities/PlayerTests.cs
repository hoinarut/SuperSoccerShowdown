using FluentAssertions;
using MyApp.Domain.Entities;
using MyApp.Domain.Enums;

namespace MyApp.Domain.Tests.Entities;

public class PlayerTests
{
    [Fact]
    public void HasValidMeasurements_WhenWeightAndHeightAreNonZero_ReturnsTrue()
    {
        var player = new Player("Valid", weight: 80, height: 180, externalResourceId: 1);

        player.HasValidMeasurements.Should().BeTrue();
    }

    [Theory]
    [InlineData(0, 180)]
    [InlineData(80, 0)]
    [InlineData(0, 0)]
    public void HasValidMeasurements_WhenWeightOrHeightIsZero_ReturnsFalse(double weight, double height)
    {
        var player = new Player("Invalid", weight, height, externalResourceId: 1);

        player.HasValidMeasurements.Should().BeFalse();
    }

    [Theory]
    [InlineData(PlayerType.Goalie)]
    [InlineData(PlayerType.Defence)]
    [InlineData(PlayerType.Offence)]
    public void SetPlayerType_SetsTypeCorrectly(PlayerType playerType)
    {
        var player = new Player("Player", weight: 80, height: 180, externalResourceId: 1);

        player.SetPlayerType(playerType);

        player.Type.Should().Be(playerType);
    }
}
