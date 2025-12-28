namespace Application.DTOs.Card;

public class CreateExampleRequest
{
    public string SentenceJapanese { get; set; } = string.Empty;
    public string SentenceMeaning { get; set; } = string.Empty;
    public string? ClozePart { get; set; }
    public string? AlternativeAnswers { get; set; }
    public int? AudioMediaId { get; set; }
}

public class UpdateExampleRequest
{
    public string? SentenceJapanese { get; set; }
    public string? SentenceMeaning { get; set; }
    public string? ClozePart { get; set; }
    public string? AlternativeAnswers { get; set; }
    public int? AudioMediaId { get; set; }
}

public class UpdateGrammarRequest
{
    public string? Structure { get; set; }
    public string? Explanation { get; set; }
    public string? Caution { get; set; }
    public string? Level { get; set; }
}
