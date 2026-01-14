using System.Security.Claims;
using BlogAPI.Controllers;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogAPI.Tests;

public class ArticlesControllerTests
{
    private readonly Mock<IArticleService> _mockArticleService;
    private readonly ArticlesController _controller;

    public ArticlesControllerTests()
    {
        _mockArticleService = new Mock<IArticleService>();
        _controller = new ArticlesController(_mockArticleService.Object);
    }

    [Fact]
    public async Task GetArticles_ReturnsOkWithArticles()
    {
        // Arrange
        var articles = new List<Article>
        {
            new Article { Id = 1, Title = "Test 1", Perex = "Perex 1", Content = "Content 1", UserId = 1 },
            new Article { Id = 2, Title = "Test 2", Perex = "Perex 2", Content = "Content 2", UserId = 1 }
        };
        _mockArticleService.Setup(s => s.GetAllArticlesAsync()).ReturnsAsync(articles);

        // Act
        var result = await _controller.GetArticles();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedArticles = Assert.IsAssignableFrom<IEnumerable<Article>>(okResult.Value);
        Assert.Equal(2, returnedArticles.Count());
    }

    [Fact]
    public async Task GetArticle_WithValidId_ReturnsOkWithArticle()
    {
        // Arrange
        var article = new Article { Id = 1, Title = "Test", Perex = "Perex", Content = "Content", UserId = 1 };
        _mockArticleService.Setup(s => s.GetArticleByIdAsync(1)).ReturnsAsync(article);

        // Act
        var result = await _controller.GetArticle(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedArticle = Assert.IsType<Article>(okResult.Value);
        Assert.Equal(1, returnedArticle.Id);
    }

    [Fact]
    public async Task GetArticle_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockArticleService.Setup(s => s.GetArticleByIdAsync(999)).ReturnsAsync((Article?)null);

        // Act
        var result = await _controller.GetArticle(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateArticle_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var userId = 1;
        var article = new Article { Title = "New Article", Perex = "Perex", Content = "Content" };
        var createdArticle = new Article { Id = 1, Title = "New Article", Perex = "Perex", Content = "Content", UserId = userId };
        
        _mockArticleService.Setup(s => s.CreateArticleAsync(article.Title, article.Perex, article.Content, userId))
            .ReturnsAsync(createdArticle);

        // Mock authenticated user
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.CreateArticle(article);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedArticle = Assert.IsType<Article>(createdResult.Value);
        Assert.Equal(1, returnedArticle.Id);
        Assert.Equal("New Article", returnedArticle.Title);
    }

    [Fact]
    public async Task UpdateArticle_WithValidOwnership_ReturnsNoContent()
    {
        // Arrange
        var userId = 1;
        var articleId = 1;
        var article = new Article { Title = "Updated", Perex = "Updated Perex", Content = "Updated Content" };
        var updatedArticle = new Article { Id = articleId, Title = "Updated", Perex = "Updated Perex", Content = "Updated Content", UserId = userId };
        
        _mockArticleService.Setup(s => s.UpdateArticleAsync(articleId, article.Title, article.Perex, article.Content, userId))
            .ReturnsAsync(updatedArticle);

        // Mock authenticated user
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.UpdateArticle(articleId, article);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateArticle_WithoutOwnership_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        var articleId = 1;
        var article = new Article { Title = "Updated", Perex = "Updated Perex", Content = "Updated Content" };
        
        _mockArticleService.Setup(s => s.UpdateArticleAsync(articleId, article.Title, article.Perex, article.Content, userId))
            .ReturnsAsync((Article?)null);

        // Mock authenticated user
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.UpdateArticle(articleId, article);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Article not found or you don't have permission to edit it.", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteArticle_WithValidOwnership_ReturnsNoContent()
    {
        // Arrange
        var userId = 1;
        var articleId = 1;
        
        _mockArticleService.Setup(s => s.DeleteArticleAsync(articleId, userId))
            .ReturnsAsync(true);

        // Mock authenticated user
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.DeleteArticle(articleId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteArticle_WithoutOwnership_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        var articleId = 1;
        
        _mockArticleService.Setup(s => s.DeleteArticleAsync(articleId, userId))
            .ReturnsAsync(false);

        // Mock authenticated user
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.DeleteArticle(articleId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Article not found or you don't have permission to delete it.", notFoundResult.Value);
    }
}
