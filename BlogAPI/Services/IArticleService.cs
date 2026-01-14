using BlogAPI.Models;

namespace BlogAPI.Services;

public interface IArticleService
{
    Task<IEnumerable<Article>> GetAllArticlesAsync();
    Task<Article?> GetArticleByIdAsync(int id);
    Task<Article> CreateArticleAsync(string title, string perex, string content, int userId);
    Task<Article?> UpdateArticleAsync(int id, string? title, string? perex, string? content, int userId);
    Task<bool> DeleteArticleAsync(int id, int userId);
}
