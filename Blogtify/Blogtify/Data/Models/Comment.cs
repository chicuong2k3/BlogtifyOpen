using Blogtify.Client.Models;

namespace Blogtify.Data.Models;

public class Comment : Entity<Guid>
{
    public Guid ContentId { get; private set; }
    public Guid UserId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public Guid? ParentCommentId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }

    private Comment() { }

    public Comment(Guid contentId, Guid userId, string content, Guid? parentId)
    {
        Id = Guid.NewGuid();
        ContentId = contentId;
        UserId = userId;
        Content = content;
        ParentCommentId = parentId;
        CreatedAt = DateTime.UtcNow;
        LastModifiedAt = null;
    }

    public void Update(string content)
    {
        Content = content;
        LastModifiedAt = DateTime.UtcNow;
    }
}
