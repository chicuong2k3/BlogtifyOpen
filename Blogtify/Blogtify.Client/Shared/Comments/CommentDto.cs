namespace Blogtify.Client.Shared.Comments;

public sealed record CommentDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid ContentId { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public Guid? ParentCommentId { get; init; }
    public int ChildrenCount { get; init; }

    public CommentDto() { }

    public CommentDto(Guid id, Guid userId, Guid contentId, string content,
                      DateTime createdAt, DateTime? lastModifiedAt,
                      Guid? parentCommentId, int childrenCount)
    {
        Id = id;
        UserId = userId;
        ContentId = contentId;
        Content = content;
        CreatedAt = createdAt;
        LastModifiedAt = lastModifiedAt;
        ParentCommentId = parentCommentId;
        ChildrenCount = childrenCount;
    }
}
