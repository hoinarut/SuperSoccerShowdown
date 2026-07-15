using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;

namespace MyApp.Infrastructure.Persistence.Ef;

public sealed class UniverseRepository(MyAppDbContext dbContext)
    : RepositoryBase<Universe>(dbContext), IUniverseRepository
{
    protected override DbSet<Universe> DbSet => DbContext.Universes;
    public Task<List<int>> GetUsedExternalResourceIds(int universeId)
    {
        return DbContext.Players.Where(x => x.Team.UniverseId == universeId).Select(x => x.ExternalResourceId).ToListAsync();
    }
}
