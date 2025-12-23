namespace Application.DTOs.Deck;

public class DeckStatsDTO
{
    public int TotalCards { get; set; }
    public int Downloads { get; set; }
    public int Learned { get; set; }
    public double Progress { get; set; }
    public int CardsDue { get; set; }
}
