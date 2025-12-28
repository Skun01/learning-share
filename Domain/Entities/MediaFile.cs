using Domain.Common;

namespace Domain.Entities;

public class MediaFile : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int UploadedByUserId { get; set; }

    public User User { get; set; } = null!;
    
    // Navigation - entities using this media
    public ICollection<Card> Cards { get; set; } = [];
    public ICollection<CardExample> CardExamples { get; set; } = [];
    public ICollection<User> Avatars { get; set; } = [];
}
