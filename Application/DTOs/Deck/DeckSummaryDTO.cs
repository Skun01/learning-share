using Application.DTOs.User;

namespace Application.DTOs.Deck;

public class DeckSummaryDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public AuthorDTO Author { get; set; } = null!;
    public DeckStatsDTO Stats { get; set; } = null!;
    public List<string> Tags { get; set; } = new();
    public bool IsPublic { get; set; }
    public int? SourceDeckId { get; set; }
    public DateTime CreatedAt { get; set; }
}
