namespace Application.DTOs.Study;

public class SubmitReviewResponse
{
    public int CardId { get; set; }
    public int OldLevel { get; set; }
    public int NewLevel { get; set; }
    public DateTime NextReviewDate { get; set; }
    public int GhostLevel { get; set; }
    public int Streak { get; set; }
    public bool IsCorrect { get; set; }
    public string Message { get; set; } = string.Empty;
}
