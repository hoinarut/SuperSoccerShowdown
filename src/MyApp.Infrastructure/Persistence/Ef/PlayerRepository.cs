using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;

namespace MyApp.Infrastructure.Persistence.Ef;

public sealed class PlayerRepository(MyAppDbContext dbContext)
    : RepositoryBase<Player>(dbContext), IPlayerRepository
{
    protected override DbSet<Player> DbSet => DbContext.Players;
    public Task<bool> Exists(string name)
    {
        return DbSet.AnyAsync(p => p.Name == name);
    }
}
