using Blogtify.Client.Shared.Services;
using Microsoft.AspNetCore.Authorization;

namespace Blogtify.Client.Auth.Requirements;

public class CommentOwnerRequirement : IAuthorizationRequirement
{
}

public class CommentOwnerAuthorizationHandler : AuthorizationHandler<CommentOwnerRequirement, Guid>
{
    private readonly ICommentService _commentService;

    public CommentOwnerAuthorizationHandler(ICommentService commentService)
    {
        _commentService = commentService;
    }
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CommentOwnerRequirement requirement,
        Guid commentId)
    {
        var result = await _commentService.GetByIdAsync(commentId);

        if (result == null)
        {
            context.Fail();
            return;
        }

        var commenterId = result.UserId;
        var userId = context.User.GetUserId();
        if (commenterId != userId)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
