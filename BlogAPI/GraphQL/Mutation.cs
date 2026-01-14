using System.Security.Claims;
using BlogAPI.Models;
using BlogAPI.Services;
using HotChocolate.Authorization;
using HotChocolate.Subscriptions;

namespace BlogAPI.GraphQL;

public class Mutation
{
    public async Task<string?> Login(
        string username,
        string password,
        [Service] IAuthService authService)
    {
        return await authService.LoginAsync(username, password);
    }

    [Authorize]
    public async Task<Article> CreateArticle(
        string title,
        string perex,
        string content,
        [Service] IArticleService articleService,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = int.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await articleService.CreateArticleAsync(title, perex, content, userId);
    }

    [Authorize]
    public async Task<Article?> UpdateArticle(
        int id,
        string? title,
        string? perex,
        string? content,
        [Service] IArticleService articleService,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = int.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await articleService.UpdateArticleAsync(id, title, perex, content, userId);
    }

    [Authorize]
    public async Task<bool> DeleteArticle(
        int id,
        [Service] IArticleService articleService,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = int.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await articleService.DeleteArticleAsync(id, userId);
    }

    [Authorize]
    public async Task<Comment> AddComment(
        int articleId,
        string content,
        [Service] ICommentService commentService,
        [Service] ITopicEventSender eventSender,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = int.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var comment = await commentService.AddCommentAsync(articleId, content, userId);

        await eventSender.SendAsync("OnCommentAdded_" + articleId, comment);

        return comment;
    }

    public async Task<Comment?> VoteComment(
        int commentId,
        int value,
        [Service] IVoteService voteService,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] ITopicEventSender eventSender)
    {
        var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var comment = await voteService.VoteAsync(commentId, value, ipAddress);

        if (comment != null)
        {
            await eventSender.SendAsync("OnVoteUpdated_" + comment.ArticleId, comment);
        }

        return comment;
    }
}
