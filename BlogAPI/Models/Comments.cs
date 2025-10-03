namespace BlogAPI.Models;

/// <summary>
/// Represents Comments entity for Article
/// </summary>
public class Comments
{
    /// <summary>
    /// Gets or sets the unique identifier for the comment.
    /// This is typically the primary key in a database.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the content of the comment.
    /// </summary>
    public string Content { get; set; } = string.Empty; // Initialize to avoid null reference warnings

    /// <summary>
    /// Gets or sets the author of the comment.
    /// </summary>
    public string Author { get; set; } = string.Empty; // Initialize to avoid null reference warnings

    /// <summary>
    /// Gets or sets the timestamp when the comment was created.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the foreign key to the associated Article.
    /// This links the comment to a specific article.
    /// </summary>
    public int ArticleId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the associated Article.
    /// This allows easy access to the parent Article object from a Comment.
    /// </summary>
    public Article Article { get; set; } 
    
    /// <summary>
    /// Gets or sets the navigation property to the associated User.
    /// This allows easy access to the parent User object from a Comment.
    /// </summary>
    public User User { get; set; } 
    public int UserId { get; set; }
}