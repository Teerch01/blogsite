using System.Security.Cryptography.X509Certificates;

namespace blogsite.Models.DTO.RequestDTO;

public class PostRequestDTO
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
}
