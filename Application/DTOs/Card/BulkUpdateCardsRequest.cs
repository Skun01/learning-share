namespace Application.DTOs.Card;

public class BulkUpdateCardsRequest
{
    public List<UpdateCardItem> Cards { get; set; } = new();
}

public class UpdateCardItem
{
    public int Id { get; set; }
    public string? Term { get; set; }
    public string? Meaning { get; set; }
    public string? Synonyms { get; set; }
    public int? ImageMediaId { get; set; }
    public string? Note { get; set; }
}
