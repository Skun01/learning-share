namespace Application.DTOs.Study;

public class SessionSummaryDTO
{
    public string SessionId { get; set; } = string.Empty;
    public int TotalReviewed { get; set; }
    public int Correct { get; set; }
    public int Incorrect { get; set; }
    public double AccuracyRate { get; set; }
    public int TimeSpentSeconds { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
}
