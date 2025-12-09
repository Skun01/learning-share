using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Deck : BaseEntity
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DeckType Type { get; set; }
    public bool IsPublic { get; set; } = false;
    public int? ParentDeckId { get; set; }
    public int TotalCards { get; set; } = 0;
    public int Downloads { get; set; } = 0;

    public User User { get; set; } = null!;
    public Deck? ParentDeck { get; set; }
    public ICollection<Deck> ChildDecks { get; set; } = [];
    public ICollection<DeckTag> DeckTags { get; set; } = [];
    public ICollection<Card> Cards { get; set; } = [];
}
