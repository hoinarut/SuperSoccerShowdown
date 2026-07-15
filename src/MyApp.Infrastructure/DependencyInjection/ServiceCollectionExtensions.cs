using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Domain.Ports;
using MyApp.Infrastructure.ExternalServices;
using MyApp.Infrastructure.Persistence.Ef;

namespace MyApp.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MyAppDbContext>(options =>
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                options.UseInMemoryDatabase(configuration.GetValue<string>("InMemoryDatabaseName") ?? "MyAppTests");
            }
            else
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        });

        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IUniverseRepository, UniverseRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddHttpClient<StarWarsUniverseDataService>();
        services.AddHttpClient<PokemonUniverseDataService>();
        services.AddScoped<IUniverseDataService, PokemonUniverseDataService>();
        services.AddScoped<IUniverseDataService, StarWarsUniverseDataService>();
        return services;
    }
}
