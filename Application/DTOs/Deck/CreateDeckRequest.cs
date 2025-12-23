namespace Application.DTOs.Deck;

public class CreateDeckRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;  // "Vocabulary" or "Grammar"
    public bool IsPublic { get; set; } = false;
    public int? ParentDeckId { get; set; }
    public List<string> Tags { get; set; } = new();
}
