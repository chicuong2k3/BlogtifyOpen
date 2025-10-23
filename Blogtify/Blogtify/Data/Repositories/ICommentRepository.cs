using Blogtify.Data.Models;

namespace Blogtify.Data.Repositories;

public interface ICommentRepository : IRepositoryBase<Comment, Guid>
{
    Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
