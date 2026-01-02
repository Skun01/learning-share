namespace Application.DTOs.Card;

public class VocabularyDetailsDTO
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
