namespace Application.DTOs.Card;

public class BulkUpdateCardsResponse
{
    public int TotalRequested { get; set; }
    public int TotalUpdated { get; set; }
    public List<CardDetailDTO> UpdatedCards { get; set; } = new();
    public List<BulkOperationError>? Errors { get; set; }
}
