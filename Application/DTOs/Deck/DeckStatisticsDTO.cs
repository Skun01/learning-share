namespace Application.DTOs.Deck;

public class DeckStatisticsDTO
{
    public int TotalDecks { get; set; }
    public int TotalCards { get; set; }
    public int TotalLearned { get; set; }
    public int TotalDue { get; set; }
    public double OverallProgress { get; set; }
    public int PublicDecks { get; set; }
    public int PrivateDecks { get; set; }
    public Dictionary<string, int> DecksByType { get; set; } = new();
}
