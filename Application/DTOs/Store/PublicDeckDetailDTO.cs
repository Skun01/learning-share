using Application.DTOs.User;

namespace Application.DTOs.Store;

public class PublicDeckDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public AuthorDTO Author { get; set; } = null!;
    public List<string> Tags { get; set; } = new();
    public int TotalCards { get; set; }
    public int Downloads { get; set; }
    public DateTime CreatedAt { get; set; }
}
