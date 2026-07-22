using System.Reflection;
using FluentAssertions;
using MyApp.Domain;
using MyApp.Domain.Entities;
using MyApp.Domain.Enums;

namespace MyApp.Domain.Tests.Entities;

public class TeamTests
{
    [Fact]
    public void AddPlayer_WhenRosterNotFull_AddsPlayer()
    {
        var team = new Team("Test Team", universeId: 1, attackersCount: 1, defendersCount: 1);
        var player = CreatePlayer("Player", weight: 80, height: 180);

        team.AddPlayer(player);

        team.Players.Should().ContainSingle().Which.Should().BeSameAs(player);
    }

    [Fact]
    public void AddPlayer_WhenRosterFull_ThrowsMaxPlayersReached()
    {
        var team = CreateTeamWithPlayers(
            attackersCount: 1,
            defendersCount: 1,
            CreatePlayer("P1", 80, 180),
            CreatePlayer("P2", 80, 181),
            CreatePlayer("P3", 80, 182),
            CreatePlayer("P4", 80, 183),
            CreatePlayer("P5", 80, 184));

        var action = () => team.AddPlayer(CreatePlayer("P6", 80, 185));

        action.Should().Throw<InvalidOperationException>()
            .WithMessage(Constants.ErrorMessages.MaxPlayersReached);
    }

    [Fact]
    public void SetPlayerTypes_WhenPlayersNull_ThrowsPlayersNotSet()
    {
        var team = new Team("Test Team", universeId: 1, attackersCount: 1, defendersCount: 1);
        SetPlayers(team, null);

        var action = () => team.SetPlayerTypes();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage(Constants.ErrorMessages.PlayersNotSet);
    }

    [Fact]
    public void SetPlayerTypes_AssignsTallestPlayerAsGoalie()
    {
        var team = CreateTeamWithPlayers(
            attackersCount: 1,
            defendersCount: 1,
            CreatePlayer("Short", weight: 80, height: 170),
            CreatePlayer("Tall", weight: 80, height: 200),
            CreatePlayer("Mid", weight: 80, height: 185),
            CreatePlayer("Mid2", weight: 80, height: 180),
            CreatePlayer("Mid3", weight: 80, height: 175));

        team.SetPlayerTypes();

        team.Players!.Single(p => p.Name == "Tall").Type.Should().Be(PlayerType.Goalie);
    }

    [Fact]
    public void SetPlayerTypes_WhenHeightsTiedForGoalie_UsesNameAsTieBreaker()
    {
        var team = CreateTeamWithPlayers(
            attackersCount: 1,
            defendersCount: 1,
            CreatePlayer("Bob", weight: 80, height: 200),
            CreatePlayer("Alice", weight: 80, height: 200),
            CreatePlayer("P3", weight: 80, height: 180),
            CreatePlayer("P4", weight: 80, height: 175),
            CreatePlayer("P5", weight: 80, height: 170));

        team.SetPlayerTypes();

        team.Players!.Single(p => p.Type == PlayerType.Goalie).Name.Should().Be("Alice");
    }

    [Fact]
    public void SetPlayerTypes_AssignsHeaviestRemainingPlayersAsDefenders()
    {
        var team = CreateTeamWithPlayers(
            attackersCount: 1,
            defendersCount: 2,
            CreatePlayer("Goalie", weight: 70, height: 200),
            CreatePlayer("Heavy", weight: 110, height: 180),
            CreatePlayer("Heavier", weight: 115, height: 180),
            CreatePlayer("Light", weight: 60, height: 170),
            CreatePlayer("Medium", weight: 80, height: 175));

        team.SetPlayerTypes();

        team.Players!.Single(p => p.Name == "Heavy").Type.Should().Be(PlayerType.Defence);
        team.Players!.Single(p => p.Name == "Heavier").Type.Should().Be(PlayerType.Defence);
    }

    [Fact]
    public void SetPlayerTypes_WhenWeightsTiedForDefenders_UsesNameAsTieBreaker()
    {
        var team = CreateTeamWithPlayers(
            attackersCount: 1,
            defendersCount: 2,
            CreatePlayer("Goalie", weight: 70, height: 200),
            CreatePlayer("Zara", weight: 110, height: 180),
            CreatePlayer("Amy", weight: 110, height: 180),
            CreatePlayer("Light", weight: 60, height: 170),
            CreatePlayer("Medium", weight: 80, height: 175));

        team.SetPlayerTypes();

        team.Players!.Single(p => p.Name == "Amy").Type.Should().Be(PlayerType.Defence);
        team.Players!.Single(p => p.Name == "Zara").Type.Should().Be(PlayerType.Defence);
    }

    [Fact]
    public void SetPlayerTypes_AssignsShortestRemainingPlayersAsAttackers()
    {
        var team = CreateTeamWithPlayers(
            attackersCount: 1,
            defendersCount: 1,
            CreatePlayer("Goalie", weight: 70, height: 200),
            CreatePlayer("Defender", weight: 110, height: 180),
            CreatePlayer("Short", weight: 80, height: 160),
            CreatePlayer("Mid", weight: 80, height: 175),
            CreatePlayer("Tall", weight: 80, height: 190));

        team.SetPlayerTypes();

        team.Players!.Single(p => p.Type == PlayerType.Offence).Name.Should().Be("Short");
    }

    [Fact]
    public void SetPlayerTypes_WhenHeightsTiedForAttackers_UsesNameAsTieBreaker()
    {
        var team = CreateTeamWithPlayers(
            attackersCount: 1,
            defendersCount: 1,
            CreatePlayer("Goalie", weight: 70, height: 200),
            CreatePlayer("Defender", weight: 110, height: 180),
            CreatePlayer("Zed", weight: 80, height: 165),
            CreatePlayer("Ann", weight: 80, height: 165),
            CreatePlayer("Tall", weight: 80, height: 190));

        team.SetPlayerTypes();

        team.Players!.Single(p => p.Type == PlayerType.Offence).Name.Should().Be("Ann");
    }

    [Fact]
    public void ValidateSetup_WhenPlayerCountIsWrong_ThrowsPlayersNotSet()
    {
        var team = CreateTeamWithPlayers(
            attackersCount: 1,
            defendersCount: 1,
            CreatePlayer("P1", 80, 180),
            CreatePlayer("P2", 80, 181));

        var action = () => team.ValidateSetup();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage(Constants.ErrorMessages.PlayersNotSet);
    }

    [Fact]
    public void ValidateSetup_WhenGoalieNotSet_ThrowsGoalieNotSet()
    {
        var team = CreateFullTeam(attackersCount: 1, defendersCount: 1);
        team.Players!.ForEach(p => p.SetPlayerType(PlayerType.Defence));

        var action = () => team.ValidateSetup();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage(Constants.ErrorMessages.GoalieNotSet);
    }

    [Fact]
    public void ValidateSetup_WhenDefendersNotSet_ThrowsDefendersNotSet()
    {
        var team = CreateFullTeam(attackersCount: 1, defendersCount: 2);
        AssignValidRoles(team, goalies: 1, defenders: 1, attackers: 1);

        var action = () => team.ValidateSetup();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage(Constants.ErrorMessages.DefendersNotSet);
    }

    [Fact]
    public void ValidateSetup_WhenAttackersNotSet_ThrowsAttackersNotSet()
    {
        var team = CreateFullTeam(attackersCount: 2, defendersCount: 1);
        AssignValidRoles(team, goalies: 1, defenders: 1, attackers: 1);

        var action = () => team.ValidateSetup();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage(Constants.ErrorMessages.AttackersNotSet);
    }

    [Fact]
    public void ValidateSetup_WhenConfigurationIsValid_DoesNotThrow()
    {
        var team = CreateFullTeam(attackersCount: 1, defendersCount: 1);

        team.SetPlayerTypes();

        var action = () => team.ValidateSetup();

        action.Should().NotThrow();
        team.Players!.Should().HaveCount(Constants.NumberOfPlayers);
        team.Players.Should().ContainSingle(p => p.Type == PlayerType.Goalie);
        team.Players.Should().ContainSingle(p => p.Type == PlayerType.Defence);
        team.Players.Should().ContainSingle(p => p.Type == PlayerType.Offence);
    }

    private static Player CreatePlayer(string name, double weight, double height, int resourceId = 1) =>
        new(name, new Weight(weight), new Height(height), resourceId);

    private static Team CreateTeamWithPlayers(int attackersCount, int defendersCount, params Player[] players)
    {
        var team = new Team("Test Team", universeId: 1, attackersCount, defendersCount);
        foreach (var player in players)
        {
            team.AddPlayer(player);
        }

        return team;
    }

    private static Team CreateFullTeam(int attackersCount, int defendersCount) =>
        CreateTeamWithPlayers(
            attackersCount,
            defendersCount,
            CreatePlayer("P1", 80, 170, 1),
            CreatePlayer("P2", 80, 175, 2),
            CreatePlayer("P3", 80, 180, 3),
            CreatePlayer("P4", 80, 185, 4),
            CreatePlayer("P5", 80, 200, 5));

    private static void AssignValidRoles(Team team, int goalies, int defenders, int attackers)
    {
        var players = team.Players!.ToList();
        var index = 0;

        for (var i = 0; i < goalies; i++, index++)
        {
            players[index].SetPlayerType(PlayerType.Goalie);
        }

        for (var i = 0; i < defenders; i++, index++)
        {
            players[index].SetPlayerType(PlayerType.Defence);
        }

        for (var i = 0; i < attackers; i++, index++)
        {
            players[index].SetPlayerType(PlayerType.Offence);
        }
    }

    private static void SetPlayers(Team team, List<Player>? players)
    {
        typeof(Team).GetProperty(nameof(Team.Players), BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(team, players);
    }
}
