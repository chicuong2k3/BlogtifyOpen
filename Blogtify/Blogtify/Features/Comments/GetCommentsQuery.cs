using Blogtify.Abstractions.Cqrs;
using Blogtify.Client.Shared.Comments;
using Dapper;
using System.Data;

namespace Blogtify.Features.Comments;

public sealed record GetCommentsQuery(
    int Count,
    DateTime TimeCursor,
    Guid ContentId,
    Guid? ParentId,
    string? SortDirection
) : IQuery<GetCommentsResponse>;

internal sealed class GetCommentsQueryHandlerDapper(IDbConnection dbConnection)
    : IQueryHandler<GetCommentsQuery, GetCommentsResponse>
{
    public async Task<GetCommentsResponse> Handle(GetCommentsQuery query, CancellationToken cancellationToken)
    {
        string parentCondition = query.ParentId.HasValue ? "\"ParentCommentId\" = @ParentId" : "\"ParentCommentId\" IS NULL";
        string compareCondition = query.SortDirection?.ToLower() switch
        {
            "asc" => "\"CreatedAt\" > @TimeCursor",
            "desc" => "\"CreatedAt\" < @TimeCursor",
            _ => "\"CreatedAt\" < @TimeCursor"
        };
        string orderBy = query.SortDirection?.ToLower() switch
        {
            "asc" => "ORDER BY \"CreatedAt\" ASC",
            "desc" => "ORDER BY \"CreatedAt\" DESC",
            _ => "ORDER BY \"CreatedAt\" DESC"
        };

        string sqlComments = $"""
            SELECT *
            FROM public."Comments"
            WHERE "ContentId" = @ContentId AND {parentCondition} AND {compareCondition}
            {orderBy}
            LIMIT @Count
            """;

        var comments = (await dbConnection.QueryAsync<CommentDto>(sqlComments, new
        {
            query.ContentId,
            query.ParentId,
            query.TimeCursor,
            query.Count
        })).ToList();

        if (!comments.Any())
            return new GetCommentsResponse(new List<CommentDto>(), null);

        var commentIds = comments.Select(c => c.Id).ToArray();
        string sqlReplyCounts =
            """
            SELECT "ParentCommentId", COUNT(*) AS "Count"
            FROM public."Comments"
            WHERE "ParentCommentId" = ANY(@Ids)
            GROUP BY "ParentCommentId"
            """;

        var replyResult = await dbConnection.QueryAsync(sqlReplyCounts, new { Ids = commentIds });

        var replyCounts = replyResult.ToDictionary(row => (Guid)row.ParentCommentId, row => (int?)row.Count);

        var updatedComments = comments.Select(c =>
        {
            replyCounts.TryGetValue(c.Id, out int? count);
            return c with { ChildrenCount = count ?? 0 };
        }).ToList();

        DateTime? timeCursor = query.SortDirection?.ToLower() switch
        {
            "asc" => updatedComments.Max(x => x.CreatedAt),
            "desc" => updatedComments.Min(x => x.CreatedAt),
            _ => updatedComments.Min(x => x.CreatedAt)
        };

        string remainCondition = query.SortDirection?.ToLower() switch
        {
            "asc" => "\"CreatedAt\" > @TimeCursor",
            "desc" => "\"CreatedAt\" < @TimeCursor",
            _ => "\"CreatedAt\" < @TimeCursor"
        };

        string sqlRemain = $"""
            SELECT 1
            FROM public."Comments"
            WHERE "ContentId" = @ContentId AND {parentCondition} AND {remainCondition}
            LIMIT 1
            """;

        var hasMoreRecords = await dbConnection.QueryFirstOrDefaultAsync<int?>(sqlRemain, new
        {
            query.ContentId,
            query.ParentId,
            query.TimeCursor
        }) != null;

        if (!hasMoreRecords)
            timeCursor = null;

        return new GetCommentsResponse(updatedComments, timeCursor);
    }
}
