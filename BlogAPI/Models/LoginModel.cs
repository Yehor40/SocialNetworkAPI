namespace BlogAPI.Models;

/// <summary>
/// Simple DTO for login request
/// </summary>
public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}