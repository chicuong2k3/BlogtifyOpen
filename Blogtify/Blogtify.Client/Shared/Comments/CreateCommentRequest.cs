namespace Blogtify.Client.Shared.Comments;

public sealed record CreateCommentRequest
(
    Guid ContentId,
    string Content,
    Guid? ParentId
);
