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

    // Thông tin tiến độ SRS
    public int SRSLevel { get; set; }
    public int GhostLevel { get; set; }
    public int Streak { get; set; }
    public DateTime? LastReviewedDate { get; set; }

    // === Thông tin SRS mở rộng ===
    public float EaseFactor { get; set; }
    public int TotalReviews { get; set; }
    public int CorrectCount { get; set; }
    public int IncorrectCount { get; set; }
    public int LapseCount { get; set; }
    public DateTime? FirstLearnedDate { get; set; }
    public bool IsSuspended { get; set; }
    public bool IsLeech { get; set; } // Computed: LapseCount >= threshold

    // Related data
    public GrammarDetailsDTO? GrammarDetails { get; set; }
    public VocabularyDetailsDTO? VocabularyDetails { get; set; }
    public List<CardExampleDTO> Examples { get; set; } = new();
}
