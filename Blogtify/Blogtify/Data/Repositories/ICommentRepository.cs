using Blogtify.Data.Models;

namespace Blogtify.Data.Repositories;

public interface ICommentRepository : IRepositoryBase<Comment, Guid>
{
    Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

internal class CommentRepository : RepositoryBase<Comment, Guid>, ICommentRepository
{
    private readonly ApplicationDbContext _dbContext;
    public CommentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Comments.FindAsync(id, cancellationToken);
    }
}