using Blogtify.Client.Shared.Comments;

namespace Blogtify.Client.Shared.Services;

public interface ICommentService
{
    Task<CommentDto> GetByIdAsync(Guid id);
}
