using Domain.Common;

namespace Domain.Entities;

public class CardExample : BaseEntity
{   
    public int CardId { get; set; }
    public string SentenceJapanese { get; set; } = string.Empty;
    public string SentenceMeaning { get; set; } = string.Empty;
    public string? ClozePart { get; set; } // phần bị che đi
    public string? AlternativeAnswers { get; set; } // các đáp án chấp nhận khác
    public string? AudioUrl { get; set; }

    // Navigation
    public Card Card { get; set; } = null!;
}
