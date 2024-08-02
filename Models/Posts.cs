namespace blogsite.Models;

public class Posts : Time
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public bool LikedByCurrentUser { get; set; }
    public int LikeCount { get; set; }
    public virtual User? User { get; set; }
}
