using FluentAssertions;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Persistence.Ef;

namespace MyApp.Infrastructure.Tests.Persistence.Ef;

public class UniverseRepositoryTests : EfRepositoryTestBase
{
    [Fact]
    public async Task GetUsedExternalResourceIds_ReturnsIdsForGivenUniverseOnly()
    {
        await SeedUniverseAsync(id: 1, name: "StarWars");
        await SeedUniverseAsync(id: 2, name: "Pokemon");
        var teamRepository = new TeamRepository(DbContext);
        var repository = new UniverseRepository(DbContext);

        var starWarsTeam = new Team("Star Wars Team", universeId: 1, attackersCount: 1, defendersCount: 1);
        starWarsTeam.AddPlayer(new Player("Luke", 80, 180, externalResourceId: 10));
        starWarsTeam.AddPlayer(new Player("Leia", 75, 170, externalResourceId: 20));
        await teamRepository.AddAsync(starWarsTeam);

        var pokemonTeam = new Team("Pokemon Team", universeId: 2, attackersCount: 1, defendersCount: 1);
        pokemonTeam.AddPlayer(new Player("Pikachu", 60, 40, externalResourceId: 30));
        await teamRepository.AddAsync(pokemonTeam);

        var starWarsResourceIds = await repository.GetUsedExternalResourceIds(universeId: 1);
        var pokemonResourceIds = await repository.GetUsedExternalResourceIds(universeId: 2);

        starWarsResourceIds.Should().BeEquivalentTo([10, 20]);
        pokemonResourceIds.Should().BeEquivalentTo([30]);
    }

    [Fact]
    public async Task GetUsedExternalResourceIds_WhenNoPlayersExist_ReturnsEmptyList()
    {
        await SeedUniverseAsync();
        var repository = new UniverseRepository(DbContext);

        var resourceIds = await repository.GetUsedExternalResourceIds(universeId: 1);

        resourceIds.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsPersistedUniverse()
    {
        await SeedUniverseAsync(id: 5, name: "StarWars");
        var repository = new UniverseRepository(DbContext);

        var universe = await repository.GetByIdAsync(5);

        universe.Should().NotBeNull();
        universe!.Name.Should().Be("StarWars");
    }
}
