using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.ToTable("MediaFiles");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.FileName).HasMaxLength(255).IsRequired();
        builder.Property(f => f.FilePath).HasMaxLength(500).IsRequired();
        builder.Property(f => f.FileType).HasMaxLength(50).IsRequired();

        // Quan hệ với User
        builder.HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
