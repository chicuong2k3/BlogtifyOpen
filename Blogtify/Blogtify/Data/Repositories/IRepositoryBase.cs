using Blogtify.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace Blogtify.Data.Repositories;

public interface IRepositoryBase<TEntity, TKey>
    where TEntity : Entity<TKey>
{
    Task<TEntity?> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

internal class RepositoryBase<TEntity, TId> : IRepositoryBase<TEntity, TId>
    where TEntity : Entity<TId>
{
    private readonly DbContext _dbContext;

    public RepositoryBase(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TEntity?> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<TEntity>().Add(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<TEntity>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync();
    }
}
