namespace BlogAPI.Models;

/// <summary>
/// Simple class for authentication validation
/// </summary>
public class AuthResponse
{
    public string Token { get; set; }
    public string Message { get; set; }
}