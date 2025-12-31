using Domain.Enums;

namespace Domain.Entities;

public class GrammarDetails
{
    public int CardId { get; set; }
    public string? Structure { get; set; }
    public string? Explanation { get; set; }
    public string? Caution { get; set; }
    public Level Level { get; set; }
    
    // Bunpro-style enhancements
    public string? FormationRules { get; set; }  // Chi tiết cách chia động từ/tính từ
    public string? Nuance { get; set; }          // Sắc thái nghĩa
    public string? UsageNotes { get; set; }      // Ghi chú cách dùng chi tiết
    public string? Register { get; set; }        // Formal/Informal/Written/Spoken

    public Card Card { get; set; } = null!;
}
