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

    // === Thuộc tính mở rộng ===
    public int Difficulty { get; set; } = 1;        // Độ khó 1-5 do creator đánh giá
    public int Priority { get; set; } = 0;          // Thứ tự học trong deck (0 = mặc định)
    public string? Tags { get; set; }               // Tags riêng cho card (comma-separated)
    public bool IsHidden { get; set; } = false;     // Ẩn card khỏi learning queue
    public int? AudioMediaId { get; set; }          // Audio pronunciation cho term
    public string? Hint { get; set; }               // Gợi ý khi user bí

    public Deck Deck { get; set; } = null!;
    public MediaFile? ImageMedia { get; set; }
    public MediaFile? AudioMedia { get; set; }
    public GrammarDetails? GrammarDetails { get; set; }
    public VocabularyDetails? VocabularyDetails { get; set; }
    public ICollection<CardExample> Examples { get; set; } = [];
}
