using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CardExampleConfiguration : IEntityTypeConfiguration<CardExample>
{
    public void Configure(EntityTypeBuilder<CardExample> builder)
    {
        builder.ToTable("CardExamples");
        builder.HasKey(ce => ce.Id);

        builder.Property(ce => ce.SentenceJapanese)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(ce => ce.SentenceMeaning)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(ce => ce.ClozePart)
            .HasMaxLength(100);
        
        builder.Property(ce => ce.AlternativeAnswers)
            .HasMaxLength(200);

        builder.Property(ce => ce.AudioUrl)
            .HasMaxLength(255);

        // Quan hệ với Card
        builder.HasOne(ce => ce.Card)
            .WithMany(c => c.Examples)
            .HasForeignKey(ce => ce.CardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
