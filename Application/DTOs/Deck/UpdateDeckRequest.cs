namespace Application.DTOs.Deck;

public class UpdateDeckRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public List<string> Tags { get; set; } = new();
}
