using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogAPI.Data;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlogAPI.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ApplicationDbContext context, ILogger<AuthService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        _logger.LogInformation("Login attempt for user: {Username}", username);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt for user: {Username}", username);
            return null;
        }

        _logger.LogInformation("Successful login for user: {Username}", username);
        return GenerateJwtToken(user);
    }

    public string GenerateJwtToken(User user)
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")!;
        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
        var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!;
        
        var key = Encoding.ASCII.GetBytes(jwtKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = jwtIssuer,
            Audience = jwtAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
