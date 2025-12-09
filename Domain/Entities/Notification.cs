using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Notification : BaseEntity
{
    public int UserId { get; set; }
    public NotificationType Type { get; set; } = NotificationType.User;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? DataJson { get; set; } // { "deckId": 10, "action": "clone" }
    public bool IsRead { get; set; } = false;

    public virtual User? User { get; set; }
}
