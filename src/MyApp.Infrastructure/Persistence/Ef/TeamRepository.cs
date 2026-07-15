using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;

namespace MyApp.Infrastructure.Persistence.Ef;

public sealed class TeamRepository(MyAppDbContext dbContext)
    : RepositoryBase<Team>(dbContext), ITeamRepository
{
    protected override DbSet<Team> DbSet => DbContext.Teams;

    public override async Task<List<Team>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Teams
            .Include(team => team.Players)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
