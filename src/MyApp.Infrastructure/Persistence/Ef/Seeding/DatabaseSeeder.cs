using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Persistence.Ef.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(MyAppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await SeedUniversesAsync(dbContext, cancellationToken);
    }

    private static async Task SeedUniversesAsync(MyAppDbContext dbContext, CancellationToken cancellationToken)
    {
        foreach (var universe in UniverseSeedData.Universes)
        {
            var exists = await dbContext.Universes
                .AnyAsync(u => u.Name == universe.Name, cancellationToken);

            if (exists)
            {
                continue;
            }

            dbContext.Universes.Add(new Universe
            {
                Name = universe.Name,
                ApiUrl = universe.ApiUrl,
                IsEnabled = universe.IsEnabled
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
