using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models;

/// <summary>
/// Represents a comment on an article.
/// </summary>
public class Comment
{
    /// <summary>
    /// Unique identifier for the comment.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Main text content of the comment.
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the comment was posted.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ID of the article this comment belongs to.
    /// </summary>
    public int ArticleId { get; set; }

    /// <summary>
    /// Associated Article object.
    /// </summary>
    public Article Article { get; set; } = null!;

    /// <summary>
    /// ID of the user who authored the comment.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Associated User (author) object.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Collection of votes associated with this comment.
    /// </summary>
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();

    /// <summary>
    /// Calculated field for the total score of the comment (sum of all votes).
    /// </summary>
    public int Score => Votes.Sum(v => v.Value);
}