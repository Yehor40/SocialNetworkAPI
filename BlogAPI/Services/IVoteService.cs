using BlogAPI.Models;

namespace BlogAPI.Services;

public interface IVoteService
{
    Task<Comment?> VoteAsync(int commentId, int value, string ipAddress);
}
