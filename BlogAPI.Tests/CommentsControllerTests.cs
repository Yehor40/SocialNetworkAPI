using System.Net;
using System.Security.Claims;
using BlogAPI.Controllers;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BlogAPI.Tests;

public class CommentsControllerTests
{
    private readonly Mock<ICommentService> _mockCommentService;
    private readonly Mock<IVoteService> _mockVoteService;
    private readonly CommentsController _controller;

    public CommentsControllerTests()
    {
        _mockCommentService = new Mock<ICommentService>();
        _mockVoteService = new Mock<IVoteService>();
        _controller = new CommentsController(_mockCommentService.Object, _mockVoteService.Object);
    }

    [Fact]
    public async Task AddComment_WithValidData_ReturnsOkWithComment()
    {
        // Arrange
        var userId = 1;
        var articleId = 1;
        var content = "This is a test comment";
        var comment = new Comment 
        { 
            Id = 1, 
            Content = content, 
            ArticleId = articleId, 
            UserId = userId,
            Timestamp = DateTime.UtcNow
        };
        
        _mockCommentService.Setup(s => s.AddCommentAsync(articleId, content, userId))
            .ReturnsAsync(comment);

        // Mock authenticated user
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.AddComment(articleId, content);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedComment = Assert.IsType<Comment>(okResult.Value);
        Assert.Equal(content, returnedComment.Content);
        Assert.Equal(articleId, returnedComment.ArticleId);
    }

    [Fact]
    public async Task Vote_WithUpvote_ReturnsOkWithUpdatedComment()
    {
        // Arrange
        var commentId = 1;
        var voteValue = 1; // Upvote
        var ipAddress = "192.168.1.1";
        var comment = new Comment 
        { 
            Id = commentId, 
            Content = "Test comment", 
            ArticleId = 1, 
            UserId = 1,
            Votes = new List<Vote> { new Vote { Value = 1, IpAddress = ipAddress } }
        };
        
        _mockVoteService.Setup(s => s.VoteAsync(commentId, voteValue, ipAddress))
            .ReturnsAsync(comment);

        // Mock HttpContext with IP address
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse(ipAddress);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.Vote(commentId, voteValue);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedComment = Assert.IsType<Comment>(okResult.Value);
        Assert.Equal(commentId, returnedComment.Id);
        Assert.Equal(1, returnedComment.Score);
    }

    [Fact]
    public async Task Vote_WithDownvote_ReturnsOkWithUpdatedComment()
    {
        // Arrange
        var commentId = 1;
        var voteValue = -1; // Downvote
        var ipAddress = "192.168.1.1";
        var comment = new Comment 
        { 
            Id = commentId, 
            Content = "Test comment", 
            ArticleId = 1, 
            UserId = 1,
            Votes = new List<Vote> { new Vote { Value = -1, IpAddress = ipAddress } }
        };
        
        _mockVoteService.Setup(s => s.VoteAsync(commentId, voteValue, ipAddress))
            .ReturnsAsync(comment);

        // Mock HttpContext with IP address
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse(ipAddress);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.Vote(commentId, voteValue);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedComment = Assert.IsType<Comment>(okResult.Value);
        Assert.Equal(commentId, returnedComment.Id);
        Assert.Equal(-1, returnedComment.Score);
    }

    [Fact]
    public async Task Vote_WithNonExistentComment_ReturnsNotFound()
    {
        // Arrange
        var commentId = 999;
        var voteValue = 1;
        var ipAddress = "192.168.1.1";
        
        _mockVoteService.Setup(s => s.VoteAsync(commentId, voteValue, ipAddress))
            .ReturnsAsync((Comment?)null);

        // Mock HttpContext with IP address
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse(ipAddress);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.Vote(commentId, voteValue);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Comment not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task Vote_WithUnknownIpAddress_UsesUnknownAsDefault()
    {
        // Arrange
        var commentId = 1;
        var voteValue = 1;
        var comment = new Comment 
        { 
            Id = commentId, 
            Content = "Test comment", 
            ArticleId = 1, 
            UserId = 1,
            Votes = new List<Vote> { new Vote { Value = 1, IpAddress = "unknown" } }
        };
        
        _mockVoteService.Setup(s => s.VoteAsync(commentId, voteValue, "unknown"))
            .ReturnsAsync(comment);

        // Mock HttpContext without IP address
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = null;
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.Vote(commentId, voteValue);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedComment = Assert.IsType<Comment>(okResult.Value);
        Assert.Equal(commentId, returnedComment.Id);
        _mockVoteService.Verify(s => s.VoteAsync(commentId, voteValue, "unknown"), Times.Once);
    }
}
