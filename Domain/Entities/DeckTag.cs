namespace Domain.Entities;

public class DeckTag
{
    public int DeckId { get; set; }
    public string TagName { get; set; } = string.Empty;
    public Deck Deck { get; set; } = null!;
}
