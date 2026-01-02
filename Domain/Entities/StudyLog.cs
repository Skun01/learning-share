using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class StudyLog : BaseEntity
{
    public int UserId { get; set; }
    public int CardId { get; set; }
    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    public bool IsCorrect { get; set; }

    // === Thông tin chi tiết review ===
    public int? ResponseTimeMs { get; set; }        // Thời gian phản hồi (ms)
    public int OldLevel { get; set; }               // Level trước khi review
    public int NewLevel { get; set; }               // Level sau khi review
    public ReviewType ReviewType { get; set; } = ReviewType.Review; // Loại review
    public string? UserAnswer { get; set; }         // Câu trả lời của user (để debug/improve)
    public int? ExampleId { get; set; }             // Example nào được dùng trong review
    public Guid? SessionId { get; set; }            // Nhóm các review trong cùng session

    public User User { get; set; } = null!;
    public Card Card { get; set; } = null!;
    public CardExample? Example { get; set; }
}
