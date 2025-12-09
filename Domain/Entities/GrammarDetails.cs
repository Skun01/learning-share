using Domain.Enums;

namespace Domain.Entities;

public class GrammarDetails
{
    public int CardId { get; set; }
    public string? Structure { get; set; }
    public string? Explanation { get; set; }
    public string? Caution { get; set; }
    public Level Level { get; set; }

    public Card Card { get; set; } = null!;
}
