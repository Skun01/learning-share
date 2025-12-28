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

        // ImageMedia FK relationship
        builder.HasOne(c => c.ImageMedia)
            .WithMany(m => m.Cards)
            .HasForeignKey(c => c.ImageMediaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
