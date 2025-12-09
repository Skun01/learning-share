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

    public User User { get; set; } = null!;
    public Card Card { get; set; } = null!;
}
