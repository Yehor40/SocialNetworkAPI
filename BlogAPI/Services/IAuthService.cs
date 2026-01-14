using BlogAPI.Models;

namespace BlogAPI.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(string username, string password);
    string GenerateJwtToken(User user);
}
