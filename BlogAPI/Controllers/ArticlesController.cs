using System.Security.Claims;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    /// <summary>
    /// Retrieves all articles with their comments and vote scores.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
    {
        var articles = await _articleService.GetAllArticlesAsync();
        return Ok(articles);
    }

    /// <summary>
    /// Retrieves a specific article by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Article>> GetArticle(int id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);
        if (article == null) return NotFound();
        return Ok(article);
    }

    /// <summary>
    /// Creates a new blog post. (Requires Authentication)
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Article>> CreateArticle(Article article)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var newArticle = await _articleService.CreateArticleAsync(article.Title, article.Perex, article.Content, userId);
        return CreatedAtAction(nameof(GetArticle), new { id = newArticle.Id }, newArticle);
    }

    /// <summary>
    /// Updates an existing blog post. (Requires Authentication and Ownership)
    /// </summary>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateArticle(int id, Article article)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var updatedArticle = await _articleService.UpdateArticleAsync(id, article.Title, article.Perex, article.Content, userId);
        
        if (updatedArticle == null) return NotFound("Article not found or you don't have permission to edit it.");

        return NoContent();
    }

    /// <summary>
    /// Deletes a blog post. (Requires Authentication and Ownership)
    /// </summary>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await _articleService.DeleteArticleAsync(id, userId);
        
        if (!success) return NotFound("Article not found or you don't have permission to delete it.");

        return NoContent();
    }
}
