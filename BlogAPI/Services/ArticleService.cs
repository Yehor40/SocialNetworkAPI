using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services;

public class ArticleService : IArticleService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ArticleService> _logger;

    public ArticleService(ApplicationDbContext context, ILogger<ArticleService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Article>> GetAllArticlesAsync()
    {
        _logger.LogInformation("Fetching all articles");
        return await _context.Articles
            .Include(a => a.Comments)
            .ThenInclude(c => c.Votes)
            .ToListAsync();
    }

    public async Task<Article?> GetArticleByIdAsync(int id)
    {
        _logger.LogInformation("Fetching article with ID: {Id}", id);
        return await _context.Articles
            .Include(a => a.Comments)
            .ThenInclude(c => c.Votes)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Article> CreateArticleAsync(string title, string perex, string content, int userId)
    {
        _logger.LogInformation("Creating new article with title: {Title} by User ID: {UserId}", title, userId);
        var article = new Article
        {
            Title = title,
            Perex = perex,
            Content = content,
            Timestamp = DateTime.UtcNow,
            UserId = userId
        };

        _context.Articles.Add(article);
        await _context.SaveChangesAsync();
        return article;
    }

    public async Task<Article?> UpdateArticleAsync(int id, string? title, string? perex, string? content, int userId)
    {
        _logger.LogInformation("Updating article with ID: {Id} by User ID: {UserId}", id, userId);
        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (article == null)
        {
            _logger.LogWarning("Article with ID: {Id} not found or user {UserId} is not the owner", id, userId);
            return null;
        }

        if (title != null) article.Title = title;
        if (perex != null) article.Perex = perex;
        if (content != null) article.Content = content;

        await _context.SaveChangesAsync();
        return article;
    }

    public async Task<bool> DeleteArticleAsync(int id, int userId)
    {
        _logger.LogInformation("Deleting article with ID: {Id} by User ID: {UserId}", id, userId);
        var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (article == null)
        {
            _logger.LogWarning("Article with ID: {Id} not found or user {UserId} is not the owner", id, userId);
            return false;
        }

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();
        return true;
    }
}
