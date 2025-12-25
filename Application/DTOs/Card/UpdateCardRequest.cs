namespace Application.DTOs.Card;

public class UpdateCardRequest
{
    public string? Type { get; set; }
    public string? Term { get; set; }
    public string? Meaning { get; set; }
    public string? Synonyms { get; set; }
    public string? ImageUrl { get; set; }
    public string? Note { get; set; }
}
