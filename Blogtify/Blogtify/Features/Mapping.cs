
using Blogtify.Client.Shared.Comments;
using Blogtify.Data.Models;
namespace Blogtify.Features;

public static class Mapping
{
    public static CommentDto ToDto(this Comment comment) =>
        new(comment.Id,
            comment.UserId,
            comment.ContentId,
            comment.Content,
            comment.CreatedAt,
            comment.LastModifiedAt,
            comment.ParentCommentId,
            0);
}
