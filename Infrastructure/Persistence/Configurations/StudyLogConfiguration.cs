using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class StudyLogConfiguration : IEntityTypeConfiguration<StudyLog>
{
    public void Configure(EntityTypeBuilder<StudyLog> builder)
    {
        builder.ToTable("StudyLogs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReviewDate)
            .HasDefaultValueSql("GETDATE()");

        // === Cấu hình các thuộc tính mới ===
        builder.Property(x => x.OldLevel)
            .HasDefaultValue(0);

        builder.Property(x => x.NewLevel)
            .HasDefaultValue(0);

        builder.Property(x => x.ReviewType)
            .HasConversion<int>();

        builder.Property(x => x.UserAnswer)
            .HasMaxLength(500);

        // Index cho SessionId để query nhanh theo session
        builder.HasIndex(x => x.SessionId);

        // Index cho thống kê theo ngày
        builder.HasIndex(x => new { x.UserId, x.ReviewDate });

        // Relationships
        builder.HasOne(x => x.User)
            .WithMany(u => u.StudyLogs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa User -> Xóa Log

        builder.HasOne(x => x.Card)
            .WithMany()
            .HasForeignKey(x => x.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Example)
            .WithMany()
            .HasForeignKey(x => x.ExampleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
