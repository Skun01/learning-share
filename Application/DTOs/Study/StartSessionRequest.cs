namespace Application.DTOs.Study;

public class StartSessionRequest
{
    public int? DeckId { get; set; }
    public string Mode { get; set; } = "review";
    public int Limit { get; set; } = 10;
}
