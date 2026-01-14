using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models;

/// <summary>
/// Represents a vote on a comment.
/// </summary>
public class Vote
{
    /// <summary>
    /// Unique identifier for the vote.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Value of the vote: +1 or -1.
    /// </summary>
    [Required]
    public int Value { get; set; }

    /// <summary>
    /// IP address of the user who voted. Used to ensure unique votes per comment.
    /// </summary>
    [Required]
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// ID of the comment being voted on.
    /// </summary>
    public int CommentId { get; set; }

    /// <summary>
    /// Associated Comment object.
    /// </summary>
    public Comment Comment { get; set; } = null!;
}
