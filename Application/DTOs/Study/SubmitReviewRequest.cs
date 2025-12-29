namespace Application.DTOs.Study;

public class SubmitReviewRequest
{
    public bool IsCorrect { get; set; }
    public int? TimeSpentMs { get; set; }
}
