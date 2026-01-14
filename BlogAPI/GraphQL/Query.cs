using BlogAPI.Models;
using BlogAPI.Services;

namespace BlogAPI.GraphQL;

public class Query
{
    public async Task<IEnumerable<Article>> GetArticles([Service] IArticleService articleService)
    {
        return await articleService.GetAllArticlesAsync();
    }

    public async Task<Article?> GetArticle(int id, [Service] IArticleService articleService)
    {
        return await articleService.GetArticleByIdAsync(id);
    }
}
