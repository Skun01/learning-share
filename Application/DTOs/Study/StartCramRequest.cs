namespace Application.DTOs.Study;

public class StartCramRequest
{
    public List<int> DeckIds { get; set; } = new();
    public string Type { get; set; } = "all";
    public int? SpecificLevel { get; set; }
    public int Limit { get; set; } = 20;
}
