namespace Application.DTOs.Card;

public class GrammarDetailsRequest
{
    public string? Structure { get; set; }
    public string? Explanation { get; set; }
    public string? Caution { get; set; }
    public string Level { get; set; } = "N5";
    public string? FormationRules { get; set; }
    public string? Nuance { get; set; }
    public string? UsageNotes { get; set; }
    public string? Register { get; set; }
}

public class CardExampleRequest
{
    public string SentenceJapanese { get; set; } = string.Empty;
    public string SentenceMeaning { get; set; } = string.Empty;
    public string? ClozePart { get; set; }
    public string? AlternativeAnswers { get; set; }
    public int? AudioMediaId { get; set; }
}

public class CreateCardRequest
{
    public string Type { get; set; } = "Vocabulary";
    public string Term { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public string? Synonyms { get; set; }
    public int? ImageMediaId { get; set; }
    public string? Note { get; set; }
    public GrammarDetailsRequest? GrammarDetails { get; set; }
    public List<CardExampleRequest>? Examples { get; set; }
}
