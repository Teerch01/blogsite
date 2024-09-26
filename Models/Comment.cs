namespace blogsite.Models;

public class Comments : Time
{
	public int Id { get; set; }
	public string? Comment { get; set; }
	public Guid UserId { get; set; }
	public int PostId { get; set; }
	public bool LikedByCurrentUser { get; set; } = false;
	public int LikeCount { get; set; }
	public virtual User? User { get; set; }
	public virtual Posts? Post { get; set; }
}