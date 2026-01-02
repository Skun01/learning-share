namespace Application.DTOs.Card;

public class CardDetailDTO
{
    public int Id { get; set; }
    public int DeckId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public string? Synonyms { get; set; }
    public int? ImageMediaId { get; set; }
    public string? ImageUrl { get; set; }
    public string? Note { get; set; }

    // === Thuộc tính mở rộng ===
    public int Difficulty { get; set; }
    public int Priority { get; set; }
    public string? Tags { get; set; }
    public bool IsHidden { get; set; }
    public int? AudioMediaId { get; set; }
    public string? AudioUrl { get; set; }
    public string? Hint { get; set; }

    public GrammarDetailsDTO? GrammarDetails { get; set; }
    public VocabularyDetailsDTO? VocabularyDetails { get; set; }
    public List<CardExampleDTO> Examples { get; set; } = new();
}
