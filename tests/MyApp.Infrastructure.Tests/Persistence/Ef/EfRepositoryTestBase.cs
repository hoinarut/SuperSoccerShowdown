using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Persistence.Ef;

namespace MyApp.Infrastructure.Tests.Persistence.Ef;

public abstract class EfRepositoryTestBase : IDisposable
{
    protected MyAppDbContext DbContext { get; }

    protected EfRepositoryTestBase()
    {
        var options = new DbContextOptionsBuilder<MyAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        DbContext = new MyAppDbContext(options);
    }

    protected async Task SeedUniverseAsync(int id = 1, string name = "StarWars")
    {
        DbContext.Universes.Add(new Universe
        {
            Id = id,
            Name = name,
            ApiUrl = "https://example.com/",
            IsEnabled = true
        });
        await DbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        DbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
