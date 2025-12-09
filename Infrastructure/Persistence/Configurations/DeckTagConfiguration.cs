using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DeckTagConfiguration : IEntityTypeConfiguration<DeckTag>
{
    public void Configure(EntityTypeBuilder<DeckTag> builder)
    {
        builder.ToTable("DeckTags");

        // Thiết lập Khóa chính phức hợp (Composite Key)
        // Một Deck không thể có 2 tag trùng tên nhau
        builder.HasKey(dt => new { dt.DeckId, dt.TagName });

        builder.Property(dt => dt.TagName)
            .HasMaxLength(50)
            .IsRequired();

        // Quan hệ với Deck
        builder.HasOne(dt => dt.Deck)
            .WithMany(d => d.DeckTags)
            .HasForeignKey(dt => dt.DeckId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
