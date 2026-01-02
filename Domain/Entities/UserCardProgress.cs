using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class UserCardProgress : BaseEntity
{
    public int UserId { get; set; }
    public int CardId { get; set; }
    public SRSLevel SRSLevel { get; set; } = SRSLevel.New;
    public DateTime? NextReviewDate { get; set; } // Null nếu chưa học bao giờ
    public DateTime? LastReviewedDate { get; set; }
    public int GhostLevel { get; set; } = 0;
    public int Streak { get; set; } = 0;

    // === Mở rộng theo dõi SRS ===
    public float EaseFactor { get; set; } = 2.5f;       // Hệ số dễ/khó SM-2 (1.3 - 2.5+)
    public int TotalReviews { get; set; } = 0;          // Tổng số lần ôn tập
    public int CorrectCount { get; set; } = 0;          // Số lần trả lời đúng
    public int IncorrectCount { get; set; } = 0;        // Số lần trả lời sai
    public int LapseCount { get; set; } = 0;            // Số lần bị reset về level thấp (phát hiện leech)
    public DateTime? FirstLearnedDate { get; set; }     // Ngày học lần đầu
    public DateTime? BurnedDate { get; set; }           // Ngày đạt trạng thái Burned
    public bool IsSuspended { get; set; } = false;      // Tạm ẩn card khó
    public DateTime? SuspendedUntil { get; set; }       // Tạm ẩn đến ngày này

    public User User { get; set; } = null!;
    public Card Card { get; set; } = null!;
}
