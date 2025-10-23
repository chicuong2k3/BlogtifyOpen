namespace Blogtify.Data.Repositories;

public interface IRepositoryBase<TEntity, TKey>
{
    Task<TEntity?> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
