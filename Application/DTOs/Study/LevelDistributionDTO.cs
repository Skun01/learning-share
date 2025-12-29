namespace Application.DTOs.Study;

public class LevelDistributionDTO
{
    public Dictionary<int, int> Distribution { get; set; } = new();
    public int TotalCards { get; set; }
    public int LearnedCards { get; set; }
    public int BurnedCards { get; set; }
}
