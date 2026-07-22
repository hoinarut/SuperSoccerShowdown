using FluentAssertions;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Persistence.Ef;

namespace MyApp.Infrastructure.Tests.Persistence.Ef;

public class PlayerRepositoryTests : EfRepositoryTestBase
{
    [Fact]
    public async Task Exists_WhenPlayerNameExists_ReturnsTrue()
    {
        await SeedUniverseAsync();
        var teamRepository = new TeamRepository(DbContext);
        var repository = new PlayerRepository(DbContext);
        var team = new Team("Team", universeId: 1, attackersCount: 1, defendersCount: 1);
        team.AddPlayer(new Player("Luke Skywalker", new Weight(80), new Height(180), externalResourceId: 1));
        await teamRepository.AddAsync(team);

        var exists = await repository.Exists("Luke Skywalker");

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task Exists_WhenPlayerNameDoesNotExist_ReturnsFalse()
    {
        await SeedUniverseAsync();
        var repository = new PlayerRepository(DbContext);

        var exists = await repository.Exists("Missing Player");

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsPersistedPlayer()
    {
        await SeedUniverseAsync();
        var teamRepository = new TeamRepository(DbContext);
        var repository = new PlayerRepository(DbContext);
        var team = new Team("Team", universeId: 1, attackersCount: 1, defendersCount: 1);
        team.AddPlayer(new Player("Leia Organa", new Weight(80), new Height(180), externalResourceId: 2));
        await teamRepository.AddAsync(team);
        var playerId = team.Players!.Single().Id;

        var player = await repository.GetByIdAsync(playerId);

        player.Should().NotBeNull();
        player!.Name.Should().Be("Leia Organa");
        player.ExternalResourceId.Should().Be(2);
    }
}
