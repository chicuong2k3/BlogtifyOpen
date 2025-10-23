using Blogtify.Client.Models;

namespace Blogtify.Data.Repositories;

public interface IContentRepository : IRepositoryBase<Content, Guid>
{
    Task<Content?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Content?> GetByTitleAsync(string title, CancellationToken cancellationToken = default);

}
