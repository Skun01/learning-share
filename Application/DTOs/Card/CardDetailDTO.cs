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
    public GrammarDetailsDTO? GrammarDetails { get; set; }
    public List<CardExampleDTO> Examples { get; set; } = new();
}
