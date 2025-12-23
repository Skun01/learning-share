namespace Application.DTOs.Deck;

public class DeckDetailDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public int? ParentDeckId { get; set; }
    public List<string> Tags { get; set; } = new();
    public int TotalCards { get; set; }
    public int Downloads { get; set; }
    public DateTime CreatedAt { get; set; }
}
