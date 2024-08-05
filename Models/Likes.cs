namespace blogsite.Models;

    public class Likes
    {
    public int Id { get; set; }
    public int PostId { get; set; }
    public Guid UserId { get; set; }
    public virtual Posts? Post { get; set; }
    public virtual User? User { get; set; }
}

