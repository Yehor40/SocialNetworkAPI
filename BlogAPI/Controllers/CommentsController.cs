using System.Security.Claims;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IVoteService _voteService;

    public CommentsController(ICommentService commentService, IVoteService voteService)
    {
        _commentService = commentService;
        _voteService = voteService;
    }

    /// <summary>
    /// Adds a comment to an article. (Requires Authentication)
    /// </summary>
    [Authorize]
    [HttpPost("{articleId}")]
    public async Task<ActionResult<Comment>> AddComment(int articleId, [FromBody] string content)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var comment = await _commentService.AddCommentAsync(articleId, content, userId);
        return Ok(comment);
    }

    /// <summary>
    /// Votes on a comment. (IP-based, unique)
    /// </summary>
    /// <param name="commentId">Target comment ID</param>
    /// <param name="value">Vote value: 1 for upvote, -1 for downvote</param>
    [HttpPost("{commentId}/vote")]
    public async Task<ActionResult<Comment>> Vote(int commentId, [FromBody] int value)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var comment = await _voteService.VoteAsync(commentId, value, ipAddress);
        
        if (comment == null) return NotFound("Comment not found.");
        
        return Ok(comment);
    }
}
