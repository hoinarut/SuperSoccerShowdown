using FluentAssertions;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Persistence.Ef;

namespace MyApp.Infrastructure.Tests.Persistence.Ef;

public class TeamRepositoryTests : EfRepositoryTestBase
{
    [Fact]
    public async Task AddAsync_PersistsTeamWithPlayers()
    {
        await SeedUniverseAsync();
        var repository = new TeamRepository(DbContext);
        var team = new Team("Dream Team", universeId: 1, attackersCount: 1, defendersCount: 1);
        team.AddPlayer(new Player("P1", 80, 180, externalResourceId: 10));
        team.AddPlayer(new Player("P2", 81, 181, externalResourceId: 11));

        var created = await repository.AddAsync(team);

        created.Id.Should().BeGreaterThan(0);
        DbContext.Teams.Should().ContainSingle();
        DbContext.Players.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_IncludesPlayers()
    {
        await SeedUniverseAsync();
        var repository = new TeamRepository(DbContext);
        var team = new Team("Dream Team", universeId: 1, attackersCount: 1, defendersCount: 1);
        team.AddPlayer(new Player("P1", 80, 180, externalResourceId: 10));
        team.AddPlayer(new Player("P2", 81, 181, externalResourceId: 11));
        await repository.AddAsync(team);

        var teams = await repository.GetAllAsync();

        teams.Should().ContainSingle();
        teams[0].Players.Should().HaveCount(2);
        teams[0].Players!.Select(p => p.Name).Should().Equal("P1", "P2");
    }
}
