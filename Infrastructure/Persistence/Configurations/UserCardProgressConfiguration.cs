using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserCardProgressConfiguration : IEntityTypeConfiguration<UserCardProgress>
{
    public void Configure(EntityTypeBuilder<UserCardProgress> builder)
    {
        builder.ToTable("UserCardProgress");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.UserId, x.CardId }).IsUnique();

        builder.Property(x => x.SRSLevel)
        .HasDefaultValue(SRSLevel.New)
        .HasConversion<int>();
        
        builder.Property(x => x.GhostLevel).HasDefaultValue(0);
        builder.Property(x => x.Streak).HasDefaultValue(0);

        builder.HasOne(x => x.User)
            .WithMany(u => u.Progresses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa User -> Xóa Progress

        builder.HasOne(x => x.Card)
            .WithMany()
            .HasForeignKey(x => x.CardId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa Card -> Xóa Progress
    }
}
