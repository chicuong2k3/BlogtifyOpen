
using Blogtify.Abstractions.Cqrs;
using Blogtify.Data.Models;
using Blogtify.Data.Repositories;

namespace Blogtify.Features.Comments;

public record DeleteCommentCommand(Guid Id) : ICommand;

internal class DeleteCommentCommandHandler(ICommentRepository commentRepository)
        : ICommandHandler<DeleteCommentCommand>
{
    public async Task Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        var comment = await commentRepository.GetByIdAsync(command.Id, cancellationToken);

        if (comment == null)
        {
            throw new NotFoundException(nameof(Comment), command.Id.ToString());
        }

        await commentRepository.RemoveAsync(comment, cancellationToken);
    }
}