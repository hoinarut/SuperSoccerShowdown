using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities;
using MyApp.Domain.Ports;

namespace MyApp.Infrastructure.Persistence.Ef;

public abstract class RepositoryBase<TEntity>(MyAppDbContext dbContext) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected MyAppDbContext DbContext { get; } = dbContext;

    protected abstract DbSet<TEntity> DbSet { get; }

    public IQueryable<TEntity> Query => DbSet.AsNoTracking().AsQueryable();

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Query.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Add(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }
}
