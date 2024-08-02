namespace blogsite.Models.DTO.ResponseDTO;

public class PostResponseDTO
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int LikeCount { get; set; }
    public string? Username { get; set; }
    public bool LikedByCurrentUser { get; set; }
}
