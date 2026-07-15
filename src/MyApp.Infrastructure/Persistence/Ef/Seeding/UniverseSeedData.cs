using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Persistence.Ef.Seeding;

public static class UniverseSeedData
{
    public static IReadOnlyList<Universe> Universes { get; } =
    [
        new()
        {
            Id = 1,
            Name = "Pokemon",
            ApiUrl = "https://pokeapi.co/api/v2/",
            IsEnabled = true
        },
        new()
        {
            Id = 2,
            Name = "StarWars",
            ApiUrl = "https://swapi.dev/api/",
            IsEnabled = true
        }
    ];
}
