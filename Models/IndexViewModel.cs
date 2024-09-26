using blogsite.Models.DTO.ResponseDTO;

namespace blogsite.Models;

public class IndexViewModel
{
    public IEnumerable<PostResponseDTO>? Posts { get; set; }
    public IEnumerable<string>? Tags { get; set; }
}
