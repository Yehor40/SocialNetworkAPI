using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CommentService> _logger;

    public CommentService(ApplicationDbContext context, ILogger<CommentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Comment> AddCommentAsync(int articleId, string content, int userId)
    {
        _logger.LogInformation("Adding comment to Article ID: {ArticleId} by User ID: {UserId}", articleId, userId);
        var comment = new Comment
        {
            ArticleId = articleId,
            Content = content,
            Timestamp = DateTime.UtcNow,
            UserId = userId
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment?> GetCommentByIdAsync(int id)
    {
        _logger.LogInformation("Fetching comment with ID: {Id}", id);
        return await _context.Comments
            .Include(c => c.Votes)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}
