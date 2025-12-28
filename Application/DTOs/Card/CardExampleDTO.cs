namespace Application.DTOs.Card;

public class CardExampleDTO
{
    public int Id { get; set; }
    public string SentenceJapanese { get; set; } = string.Empty;
    public string SentenceMeaning { get; set; } = string.Empty;
    public string? ClozePart { get; set; }
    public string? AlternativeAnswers { get; set; }
    public int? AudioMediaId { get; set; }
    public string? AudioUrl { get; set; }
}
