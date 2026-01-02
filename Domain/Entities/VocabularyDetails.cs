using Domain.Enums;

namespace Domain.Entities;

public class VocabularyDetails
{
    public int CardId { get; set; }
    public string? Reading { get; set; }      // Hiragana/Katakana reading (e.g., たべる for 食べる)
    public string? PartOfSpeech { get; set; } // Noun, Verb, い-Adjective, な-Adjective, Adverb, etc.
    public string? Pitch { get; set; }        // Pitch accent pattern

    // === Mở rộng theo Bunpro/WaniKani ===
    public Level? JLPTLevel { get; set; }           // N5-N1
    public int? Frequency { get; set; }             // Tần suất sử dụng (1 = phổ biến nhất)
    public int? WaniKaniLevel { get; set; }         // Level WaniKani nếu có
    public string? Transitivity { get; set; }       // Transitive/Intransitive (cho động từ)
    public string? VerbGroup { get; set; }          // Group 1/2/3, Ichidan/Godan
    public string? AdjectiveType { get; set; }      // い-adj, な-adj
    public string? CommonCollocations { get; set; } // Các cụm từ hay đi kèm
    public string? Antonyms { get; set; }           // Từ trái nghĩa
    public string? KanjiComponents { get; set; }    // Các kanji thành phần

    public Card Card { get; set; } = null!;
}
