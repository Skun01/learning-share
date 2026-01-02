namespace Application.DTOs.Card;

public class CardSummaryDTO
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public int? ImageMediaId { get; set; }
    public string? ImageUrl { get; set; }
    public bool HasExamples { get; set; }
    public bool HasGrammarDetails { get; set; }
    public bool HasVocabularyDetails { get; set; }

    // === Thuộc tính mở rộng ===
    public int Difficulty { get; set; }
    public int Priority { get; set; }
    public string? Tags { get; set; }
    public bool IsHidden { get; set; }
}
