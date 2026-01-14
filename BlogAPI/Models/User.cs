namespace BlogAPI.Models;

/// <summary>
/// Represents User which is author of articles and comments
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// This is typically the primary key in a database.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the username of the user.
    /// This should be unique and is often used for login.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// This should also typically be unique.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the hashed password for the user.
    /// </summary>
    public string PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets the collection of articles authored by this user.
    /// This establishes a one-to-many relationship: one User can author many Articles.
    /// </summary>
    public ICollection<Article> Articles { get; set; } = new List<Article>();

    /// <summary>
    /// Gets or sets the collection of comments made by this user.
    /// This establishes a one-to-many relationship: one User can make many Comments.
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}