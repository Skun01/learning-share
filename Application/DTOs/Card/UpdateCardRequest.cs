namespace Application.DTOs.Card;

public class UpdateCardRequest
{
    public string? Type { get; set; }
    public string? Term { get; set; }
    public string? Meaning { get; set; }
    public string? Synonyms { get; set; }
    public int? ImageMediaId { get; set; }
    public string? Note { get; set; }

    // === Thuộc tính mở rộng ===
    public int? Difficulty { get; set; }
    public int? Priority { get; set; }
    public string? Tags { get; set; }
    public bool? IsHidden { get; set; }
    public int? AudioMediaId { get; set; }
    public string? Hint { get; set; }

    public GrammarDetailsRequest? GrammarDetails { get; set; }
    public VocabularyDetailsRequest? VocabularyDetails { get; set; }
}
