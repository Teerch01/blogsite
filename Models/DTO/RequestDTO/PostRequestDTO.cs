namespace blogsite.Models.DTO.RequestDTO;

public class PostRequestDTO
{
	public int Id { get; set; }
	public string? Tag {get; set;}
	public string? Title { get; set; }
	public string? Content { get; set; }
	public IFormFile? ImageFile { get; set; }
}
