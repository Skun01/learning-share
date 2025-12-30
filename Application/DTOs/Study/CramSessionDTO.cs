namespace Application.DTOs.Study;

public class CramSessionDTO
{
    public string SessionId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int TotalCards { get; set; }
    public int CurrentIndex { get; set; }
    public int Correct { get; set; }
    public int Incorrect { get; set; }
    public DateTime StartedAt { get; set; }
    public StudyCardDTO? CurrentCard { get; set; }
    public List<int> Queue { get; set; } = new();
}
