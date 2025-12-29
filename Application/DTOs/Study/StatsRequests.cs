namespace Application.DTOs.Study;

public class GetHeatmapRequest
{
    public int Year { get; set; } = DateTime.UtcNow.Year;
}

public class GetForecastRequest
{
    public int Days { get; set; } = 7;
}

public class GetAccuracyRequest
{
    public string Period { get; set; } = "week";
}
