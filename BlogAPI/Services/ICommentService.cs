using BlogAPI.Models;

namespace BlogAPI.Services;

public interface ICommentService
{
    Task<Comment> AddCommentAsync(int articleId, string content, int userId);
    Task<Comment?> GetCommentByIdAsync(int id);
}
