using Blogtify.Abstractions.Cqrs;
using Blogtify.Client.Shared.Comments;
using Blogtify.Data.Models;
using Blogtify.Data.Repositories;

namespace Blogtify.Features.Comments;

public sealed record GetCommentByIdQuery(
    Guid Id
) : IQuery<CommentDto>;

internal sealed class GetCommentByIdQueryHandler(ICommentRepository commentRepository)
    : IQueryHandler<GetCommentByIdQuery, CommentDto>
{
    public async Task<CommentDto> Handle(GetCommentByIdQuery query, CancellationToken cancellationToken)
    {
        var comment = await commentRepository.GetByIdAsync(query.Id, cancellationToken);

        if (comment == null)
        {
            throw new NotFoundException(nameof(Comment), query.Id.ToString());
        }

        return comment.ToDto();
    }
}
