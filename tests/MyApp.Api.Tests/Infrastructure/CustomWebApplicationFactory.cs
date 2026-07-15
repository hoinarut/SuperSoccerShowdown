using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.ExternalServices;
using MyApp.Infrastructure.Persistence.Ef;
using MyApp.Infrastructure.Persistence.Ef.Seeding;

namespace MyApp.Api.Tests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["UseInMemoryDatabase"] = "true",
                ["InMemoryDatabaseName"] = _databaseName
            });
        });

        builder.ConfigureTestServices(services =>
        {
            var handler = new ExternalApiMockHandler();
            services.AddSingleton(handler);

            services.AddHttpClient<StarWarsUniverseDataService>()
                .ConfigurePrimaryHttpMessageHandler(_ => handler);

            services.AddHttpClient<PokemonUniverseDataService>()
                .ConfigurePrimaryHttpMessageHandler(_ => handler);
        });
    }

    public void SeedUniverses()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
        dbContext.Database.EnsureCreated();

        if (dbContext.Universes.Any())
        {
            return;
        }

        foreach (var universe in UniverseSeedData.Universes)
        {
            dbContext.Universes.Add(new Universe
            {
                Name = universe.Name,
                ApiUrl = universe.ApiUrl,
                IsEnabled = universe.IsEnabled
            });
        }

        dbContext.SaveChanges();
    }

    public int GetStarWarsUniverseId()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
        return dbContext.Universes.Single(u => u.Name == "StarWars").Id;
    }
}
