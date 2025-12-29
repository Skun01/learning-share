namespace Application.DTOs.Study;

public class GetAvailableReviewsRequest
{
    public int? DeckId { get; set; }
    public int Limit { get; set; } = 20;
}
