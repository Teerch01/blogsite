namespace blogsite.Models;

public class User : Time
{
	public Guid Id { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public string? Email { get; set; }
	public string? Username { get; set; }
	public string? Password { get; set; }
	public virtual ICollection<Posts>? Posts { get; set; }
}
