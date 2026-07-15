using MyApp.Domain.Entities;

namespace MyApp.Domain.Ports;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    IQueryable<TEntity> Query { get; }

    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
}
