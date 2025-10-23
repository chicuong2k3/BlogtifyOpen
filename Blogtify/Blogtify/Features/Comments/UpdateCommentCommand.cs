using Blogtify.Abstractions.Cqrs;
using Blogtify.Client.Shared.Comments;
using Blogtify.Data.Models;
using Blogtify.Data.Repositories;

namespace Blogtify.Features.Comments;

public record UpdateCommentCommand(
    Guid Id,
    string Content) : ICommand<CommentDto>;

internal class UpdateCommentCommandHandler(ICommentRepository commentRepository)
    : ICommandHandler<UpdateCommentCommand, CommentDto>
{
    public async Task<CommentDto> Handle(UpdateCommentCommand command, CancellationToken cancellationToken)
    {
        var comment = await commentRepository.GetByIdAsync(command.Id, cancellationToken);

        if (comment == null)
        {
            throw new NotFoundException(nameof(Comment), command.Id.ToString());
        }


        comment.Update(command.Content);
        await commentRepository.SaveChangesAsync(cancellationToken);

        return comment.ToDto();
    }
}