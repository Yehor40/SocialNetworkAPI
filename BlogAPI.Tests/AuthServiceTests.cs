using BlogAPI.Data;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlogAPI.Tests;

public class AuthServiceTests
{
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthService _authService;
    private readonly ApplicationDbContext _context;

    public AuthServiceTests()
    {
        _mockLogger = new Mock<ILogger<AuthService>>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        // Set up IConfiguration mock
        _mockConfiguration.Setup(c => c["JWT_KEY"]).Returns("ThisIsAVerySecretKeyForMyBloggingEngine2026");
        _mockConfiguration.Setup(c => c["JWT_ISSUER"]).Returns("BlogAPI");
        _mockConfiguration.Setup(c => c["JWT_AUDIENCE"]).Returns("BlogApp");

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "AuthTest")
            .Options;
        _context = new ApplicationDbContext(options);

        _authService = new AuthService(_context, _mockLogger.Object, _mockConfiguration.Object);
    }

    [Fact]
    public void GenerateJwtToken_ReturnsValidToken()
    {
        // Arrange
        var user = new User { UserId = 1, Username = "testuser" };

        // Act
        var token = _authService.GenerateJwtToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var password = "Password123!";
        var user = new User 
        { 
            UserId = 2, 
            Username = "validuser", 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Email = "test@example.com"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var token = await _authService.LoginAsync("validuser", password);

        // Assert
        Assert.NotNull(token);
    }
}
