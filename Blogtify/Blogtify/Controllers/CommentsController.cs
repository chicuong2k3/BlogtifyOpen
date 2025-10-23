using Blogtify.Client.Auth;
using Blogtify.Client.Shared.Comments;
using Blogtify.Features.Comments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blogtify.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IAuthorizationService _authorizationService;

    public CommentsController(
        ISender sender,
        IAuthorizationService authorizationService)
    {
        _sender = sender;
        _authorizationService = authorizationService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CommentDto>> Create([FromBody] CreateCommentRequest request)
    {
        var command = new CreateCommentCommand(
            request.ContentId,
            User.GetUserId(),
            request.Content,
            request.ParentId);
        var response = await _sender.Send(command);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetCommentsResponse>> GetMany([FromQuery] GetCommentsQuery query)
    {
        var response = await _sender.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetOne(Guid id)
    {
        var response = await _sender.Send(new GetCommentByIdQuery(id));
        return Ok(response);
    }


    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommentRequest request)
    {
        var result = await _authorizationService.AuthorizeAsync(User, id, PolicyConstants.CanManageComment);
        if (!result.Succeeded)
        {
            return Forbid();
        }

        var command = new UpdateCommentCommand(
            id,
            request.Content);
        var response = await _sender.Send(command);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _authorizationService.AuthorizeAsync(User, id, PolicyConstants.CanManageComment);
        if (!result.Succeeded)
        {
            return Forbid();
        }

        await _sender.Send(new DeleteCommentCommand(id));
        return NoContent();
    }
}
