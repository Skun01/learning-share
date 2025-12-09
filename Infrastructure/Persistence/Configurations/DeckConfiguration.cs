using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DeckConfiguration : IEntityTypeConfiguration<Deck>
{
    public void Configure(EntityTypeBuilder<Deck> builder)
    {
        builder.ToTable("Decks");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(d => d.Type)
            .HasMaxLength(20)
            .HasConversion<string>(); // Lưu "Vocabulary", "Grammar" vào DB

        builder.Property(d => d.IsPublic)
            .HasDefaultValue(false);

        builder.Property(d => d.TotalCards)
            .HasDefaultValue(0);

        builder.Property(d => d.Downloads)
            .HasDefaultValue(0);

        // Quan hệ với User
        builder.HasOne(d => d.User)
            .WithMany(u => u.Decks)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Quan hệ Self-referencing (Cha - Con)
        builder.HasOne(d => d.ParentDeck)
            .WithMany(d => d.ChildDecks)
            .HasForeignKey(d => d.ParentDeckId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
