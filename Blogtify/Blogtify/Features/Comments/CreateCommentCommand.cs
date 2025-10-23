using Blogtify.Abstractions.Cqrs;
using Blogtify.Client.Models;
using Blogtify.Client.Shared.Comments;
using Blogtify.Data.Models;
using Blogtify.Data.Repositories;


namespace Blogtify.Features.Comments;

public record CreateCommentCommand(
    Guid ContentId,
    Guid UserId,
    string Content,
    Guid? ParentId = null) : ICommand<CommentDto>;

internal class CreateCommentCommandHandler(
    ICommentRepository commentRepository,
    IContentRepository contentRepository)
    : ICommandHandler<CreateCommentCommand, CommentDto>
{
    public async Task<CommentDto> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
    {
        var content = await contentRepository.GetByIdAsync(command.ContentId, cancellationToken);

        if (content == null)
        {
            throw new NotFoundException(nameof(Content), command.ContentId.ToString());
        }

        if (command.ParentId.HasValue)
        {
            var parentComment = await commentRepository.GetByIdAsync(command.ParentId.Value, cancellationToken);

            if (parentComment == null)
            {
                throw new NotFoundException(nameof(Comment), command.ParentId.Value.ToString());
            }

            if (parentComment.ContentId != command.ContentId)
            {
                throw new BadRequestException("Parent comment does not belong to the same content.");
            }
        }


        // check spam
        //var checkCommentResult = await akismetClient.CheckCommentAsync(new AkismetComment
        //{
        //    CommentContent = command.Content,
        //});

        //if (checkCommentResult.IsSpam)
        //{
        //    return Result.Error("Comment is spam.");
        //}


        var comment = new Comment(command.ContentId, command.UserId, command.Content, command.ParentId);

        await commentRepository.AddAsync(comment, cancellationToken);

        return comment.ToDto();
    }
}
