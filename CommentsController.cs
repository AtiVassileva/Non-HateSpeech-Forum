public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; }
    public Role Role { get; set; }
}


[ApiController]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly ForumContext _context;
    private readonly SentimentAnalysisService _sentimentAnalysisService;

    public CommentsController(ForumContext context, SentimentAnalysisService sentimentAnalysisService)
    {
        _context = context;
        _sentimentAnalysisService = sentimentAnalysisService;
    }

    [HttpPost]
    public IActionResult PostComment([FromBody] Comment comment)
    {
        comment.CreatedAt = DateTime.UtcNow;
        comment.Sentiment = _sentimentAnalysisService.PredictSentiment(comment.Content);
        comment.IsFlagged = comment.Sentiment == Sentiment.Negative;

        _context.Comments.Add(comment);
        _context.SaveChanges();

        return Ok(comment);
    }

    [HttpGet]
    public IActionResult GetComments()
    {
        return Ok(_context.Comments.Include(c => c.User).ToList());
    }

    [HttpGet("flagged")]
    public IActionResult GetFlaggedComments()
    {
        return Ok(_context.Comments.Include(c => c.User).Where(c => c.IsFlagged).ToList());
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Moderator,Administrator")]
    public IActionResult DeleteComment(int id)
    {
        var comment = _context.Comments.Find(id);
        if (comment == null)
        {
            return NotFound();
        }

        _context.Comments.Remove(comment);
        _context.SaveChanges();

        return NoContent();
    }
}