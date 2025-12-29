namespace Application.DTOs.Study;

public class EndSessionRequest
{
    public string SessionId { get; set; } = string.Empty;
    public int Correct { get; set; }
    public int Incorrect { get; set; }
    public DateTime StartedAt { get; set; }
}
