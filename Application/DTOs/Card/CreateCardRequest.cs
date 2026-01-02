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

public class VocabularyDetailsRequest
{
    public string? Reading { get; set; }
    public string? PartOfSpeech { get; set; }
    public string? Pitch { get; set; }

    // === Thuộc tính mở rộng ===
    public string? JLPTLevel { get; set; }          // N5-N1
    public int? Frequency { get; set; }             // Tần suất sử dụng
    public int? WaniKaniLevel { get; set; }         // Level WaniKani
    public string? Transitivity { get; set; }       // Transitive/Intransitive
    public string? VerbGroup { get; set; }          // Group 1/2/3
    public string? AdjectiveType { get; set; }      // い-adj, な-adj
    public string? CommonCollocations { get; set; } // Các cụm từ hay đi kèm
    public string? Antonyms { get; set; }           // Từ trái nghĩa
    public string? KanjiComponents { get; set; }    // Các kanji thành phần
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

    // === Thuộc tính mở rộng ===
    public int Difficulty { get; set; } = 1;
    public int Priority { get; set; } = 0;
    public string? Tags { get; set; }
    public bool IsHidden { get; set; } = false;
    public int? AudioMediaId { get; set; }
    public string? Hint { get; set; }

    public GrammarDetailsRequest? GrammarDetails { get; set; }
    public VocabularyDetailsRequest? VocabularyDetails { get; set; }
    public List<CardExampleRequest>? Examples { get; set; }
}
