using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("Cards");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Type)
            .HasMaxLength(20)
            .HasConversion<string>(); // Lưu "Vocab", "Grammar"

        builder.Property(c => c.Term)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Meaning)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.Synonyms)
            .HasMaxLength(500);

        // === Cấu hình các thuộc tính mới ===
        builder.Property(c => c.Difficulty)
            .HasDefaultValue(1);

        builder.Property(c => c.Priority)
            .HasDefaultValue(0);

        builder.Property(c => c.Tags)
            .HasMaxLength(500);

        builder.Property(c => c.IsHidden)
            .HasDefaultValue(false);

        builder.Property(c => c.Hint)
            .HasMaxLength(500);

        // Index cho Priority để sort nhanh
        builder.HasIndex(c => new { c.DeckId, c.Priority });

        // Quan hệ với Deck
        builder.HasOne(c => c.Deck)
            .WithMany(d => d.Cards)
            .HasForeignKey(c => c.DeckId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa Deck -> Xóa hết Cards bên trong

        // Quan hệ 1-1 với GrammarDetails
        builder.HasOne(c => c.GrammarDetails)
            .WithOne(g => g.Card)
            .HasForeignKey<GrammarDetails>(g => g.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Quan hệ 1-1 với VocabularyDetails
        builder.HasOne(c => c.VocabularyDetails)
            .WithOne(v => v.Card)
            .HasForeignKey<VocabularyDetails>(v => v.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        // ImageMedia FK relationship
        builder.HasOne(c => c.ImageMedia)
            .WithMany(m => m.Cards)
            .HasForeignKey(c => c.ImageMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        // AudioMedia FK relationship
        builder.HasOne(c => c.AudioMedia)
            .WithMany()
            .HasForeignKey(c => c.AudioMediaId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
