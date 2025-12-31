namespace Application.DTOs.Card;

public class GrammarDetailsDTO
{
    public string? Structure { get; set; }
    public string? Explanation { get; set; }
    public string? Caution { get; set; }
    public string Level { get; set; } = string.Empty;
    public string? FormationRules { get; set; }
    public string? Nuance { get; set; }
    public string? UsageNotes { get; set; }
    public string? Register { get; set; }
}
