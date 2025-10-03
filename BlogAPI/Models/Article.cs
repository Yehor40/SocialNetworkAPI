using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models;

/// <summary>
/// Represents Article entity
/// </summary>
public class Article
{
    /// <summary>
    /// Gets or sets the unique identifier for the article.
    /// This is typically the primary key in a database.
    /// </summary>
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the article.
    /// This is a concise heading for the content.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the perex (summary or excerpt) of the article.
    /// This provides a brief overview before the full content.
    /// </summary>
    public string Perex { get; set; }

    /// <summary>
    /// Gets or sets the main content of the article.
    /// This holds the full body of the article.
    /// </summary>
    public string Content { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when the article was created or last updated.
    /// This provides chronological information.
    /// </summary>
    public DateTime Timestamp { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of comments associated with this article.
    /// This establishes a one-to-many relationship: one Article can have many Comments.
    /// </summary>
    public ICollection<Comments> Comments { get; set; } = new List<Comments>();
    
    /// <summary>
    /// Gets or sets the collection of users associated with the article.
    /// This establishes a one-to-many relationship: one User can have many Articles.
    /// </summary>
    public User User { get; set; } 
    public int UserId { get; set; }

}
