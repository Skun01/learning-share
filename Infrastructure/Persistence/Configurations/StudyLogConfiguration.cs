using Domain.Entities;
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

        // Relationships
        builder.HasOne(x => x.User)
            .WithMany(u => u.StudyLogs) 
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa User -> Xóa Log
        
        builder.HasOne(x => x.Card)
            .WithMany()
            .HasForeignKey(x => x.CardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
