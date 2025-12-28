using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Card : BaseEntity
{
    public int DeckId { get; set; }
    public CardType Type { get; set; }
    public string Term { get; set; } = string.Empty; // Từ vựng / Cấu trúc
    public string Meaning { get; set; } = string.Empty; // Nghĩa
    public string? Synonyms { get; set; }
    public int? ImageMediaId { get; set; }
    public string? Note { get; set; }

    public Deck Deck { get; set; } = null!;
    public MediaFile? ImageMedia { get; set; }
    public GrammarDetails? GrammarDetails { get; set; }
    public ICollection<CardExample> Examples { get; set; } = [];
}
