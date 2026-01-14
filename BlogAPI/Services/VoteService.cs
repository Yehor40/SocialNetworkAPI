using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services;

public class VoteService : IVoteService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VoteService> _logger;

    public VoteService(ApplicationDbContext context, ILogger<VoteService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Comment?> VoteAsync(int commentId, int value, string ipAddress)
    {
        _logger.LogInformation("Processing vote for Comment ID: {CommentId} with value: {Value} from IP: {IpAddress}", commentId, value, ipAddress);
        
        if (value != 1 && value != -1)
        {
            _logger.LogError("Invalid vote value received: {Value}", value);
            throw new ArgumentException("Value must be 1 or -1");
        }

        var existingVote = await _context.Votes
            .FirstOrDefaultAsync(v => v.CommentId == commentId && v.IpAddress == ipAddress);

        if (existingVote != null)
        {
            if (existingVote.Value == value)
            {
                _logger.LogInformation("Removing existing vote for Comment ID: {CommentId} from IP: {IpAddress}", commentId, ipAddress);
                _context.Votes.Remove(existingVote);
            }
            else
            {
                _logger.LogInformation("Changing vote value for Comment ID: {CommentId} from IP: {IpAddress} to {Value}", commentId, ipAddress, value);
                existingVote.Value = value;
            }
        }
        else
        {
            _logger.LogInformation("Adding new vote for Comment ID: {CommentId} from IP: {IpAddress} with value: {Value}", commentId, ipAddress, value);
            _context.Votes.Add(new Vote
            {
                CommentId = commentId,
                Value = value,
                IpAddress = ipAddress
            });
        }

        await _context.SaveChangesAsync();

        return await _context.Comments
            .Include(c => c.Votes)
            .FirstOrDefaultAsync(c => c.Id == commentId);
    }
}
