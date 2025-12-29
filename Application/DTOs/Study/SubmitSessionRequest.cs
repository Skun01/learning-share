namespace Application.DTOs.Study;

public class SubmitSessionRequest
{
    public string SessionId { get; set; } = string.Empty;
    public int CardId { get; set; }
    public bool IsCorrect { get; set; }
    public int Correct { get; set; }
    public int Incorrect { get; set; }
    public List<int> RemainingQueue { get; set; } = new();
    public DateTime StartedAt { get; set; }
}
