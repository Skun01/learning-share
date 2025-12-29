using Application.DTOs.Card;

namespace Application.DTOs.Study;

/// <summary>
/// Card data for study session (review or new lesson)
/// </summary>
public class StudyCardDTO
{
    public int CardId { get; set; }
    public int DeckId { get; set; }
    public string DeckName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public string? Synonyms { get; set; }
    public int? ImageMediaId { get; set; }
    public string? ImageUrl { get; set; }
    public string? Note { get; set; }
    
    // SRS Progress info
    public int SRSLevel { get; set; }
    public int GhostLevel { get; set; }
    public int Streak { get; set; }
    public DateTime? LastReviewedDate { get; set; }
    
    // Related data
    public GrammarDetailsDTO? GrammarDetails { get; set; }
    public List<CardExampleDTO> Examples { get; set; } = new();
}
