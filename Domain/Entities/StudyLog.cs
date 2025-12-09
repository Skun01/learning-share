using Domain.Common;

namespace Domain.Entities;

public class StudyLog : BaseEntity
{
    public int UserId { get; set; }
    public int CardId { get; set; }
    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    public bool IsCorrect { get; set; }

    public User User { get; set; } = null!;
    public Card Card { get; set; } = null!;
}
