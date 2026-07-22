using FluentAssertions;
using MyApp.Domain.Entities;
using MyApp.Domain.Enums;

namespace MyApp.Domain.Tests.Entities;

public class PlayerTests
{
    [Fact]
    public void HasValidMeasurements_WhenWeightAndHeightAreNonZero_ReturnsTrue()
    {
        var player = new Player("Valid", new Weight(80), new Height(180), externalResourceId: 1);

        player.HasValidMeasurements.Should().BeTrue();
    }

    [Theory]
    [InlineData(0, 180)]
    [InlineData(80, 0)]
    [InlineData(0, 0)]
    public void HasValidMeasurements_WhenWeightOrHeightIsZero_ReturnsFalse(double weight, double height)
    {
        var player = new Player("Invalid", new Weight(weight), new Height(height), externalResourceId: 1);

        player.HasValidMeasurements.Should().BeFalse();
    }

    [Theory]
    [InlineData(PlayerType.Goalie)]
    [InlineData(PlayerType.Defence)]
    [InlineData(PlayerType.Offence)]
    public void SetPlayerType_SetsTypeCorrectly(PlayerType playerType)
    {
        var player = new Player("Player", new Weight(80), new Height(180), externalResourceId: 1);

        player.SetPlayerType(playerType);

        player.Type.Should().Be(playerType);
    }

    [Fact]
    public void Stats_ContainsWeightAndHeight()
    {
        var player = new Player("Player", new Weight(80), new Height(180), externalResourceId: 1);

        player.Stats.Should().HaveCount(2);
        player.Stats.Should().ContainSingle(stat => stat is Weight && stat.Value == 80);
        player.Stats.Should().ContainSingle(stat => stat is Height && stat.Value == 180);
    }

    [Fact]
    public void AddStat_AddsNewPlayerStat()
    {
        var player = new Player("Player", new Weight(80), new Height(180), externalResourceId: 1);
        var speed = new Speed(90);

        player.AddStat(speed);

        player.Stats.Should().Contain(speed);
        player.HasValidMeasurements.Should().BeTrue();
    }

    [Fact]
    public void AddStat_WhenStatTypeAlreadyExists_Throws()
    {
        var player = new Player("Player", new Weight(80), new Height(180), externalResourceId: 1);

        var action = () => player.AddStat(new Weight(90));

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Player already has a Weight stat.");
    }

    private sealed class Speed : PlayerStat
    {
        public Speed(double value) : base(value)
        {
        }
    }
}
