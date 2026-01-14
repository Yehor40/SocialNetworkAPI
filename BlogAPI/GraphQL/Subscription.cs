using BlogAPI.Models;

namespace BlogAPI.GraphQL;

public class Subscription
{
    [Subscribe]
    [Topic("OnCommentAdded_{articleId}")]
    public Comment OnCommentAdded(int articleId, [EventMessage] Comment comment) => comment;

    [Subscribe]
    [Topic("OnVoteUpdated_{articleId}")]
    public Comment OnVoteUpdated(int articleId, [EventMessage] Comment comment) => comment;
}
