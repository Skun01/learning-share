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
}
