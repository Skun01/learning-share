namespace Application.DTOs.Study;

public class SubmitReviewRequest
{
    public int CardId { get; set; }
    public bool IsCorrect { get; set; }
    public int? TimeSpentMs { get; set; }
}
