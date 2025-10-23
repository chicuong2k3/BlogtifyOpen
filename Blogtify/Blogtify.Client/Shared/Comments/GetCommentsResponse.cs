namespace Blogtify.Client.Shared.Comments;

public record GetCommentsResponse
{
    public IEnumerable<CommentDto> Comments { get; init; } = [];
    public DateTime? TimeCursor { get; init; }

    public GetCommentsResponse()
    {
    }

    public GetCommentsResponse(IEnumerable<CommentDto> comments, DateTime? timeCursor)
    {
        Comments = comments;
        TimeCursor = timeCursor;
    }
}

