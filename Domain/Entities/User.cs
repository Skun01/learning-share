using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Learner;
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    
    public UserSettings? Settings { get; set; }
    public ICollection<Deck> Decks { get; set; } = [];
    public ICollection<UserCardProgress> Progresses { get; set; } = new List<UserCardProgress>();
    public ICollection<StudyLog> StudyLogs { get; set; } = new List<StudyLog>();
}
