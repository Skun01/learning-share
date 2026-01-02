using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class VocabularyDetailsConfiguration : IEntityTypeConfiguration<VocabularyDetails>
{
    public void Configure(EntityTypeBuilder<VocabularyDetails> builder)
    {
        builder.ToTable("VocabularyDetails");
        builder.HasKey(v => v.CardId);

        builder.Property(v => v.Reading)
            .HasMaxLength(200);

        builder.Property(v => v.PartOfSpeech)
            .HasMaxLength(50);

        builder.Property(v => v.Pitch)
            .HasMaxLength(50);

        // === Cấu hình các thuộc tính mới ===
        builder.Property(v => v.JLPTLevel)
            .HasConversion<int?>();

        builder.Property(v => v.Transitivity)
            .HasMaxLength(50);

        builder.Property(v => v.VerbGroup)
            .HasMaxLength(50);

        builder.Property(v => v.AdjectiveType)
            .HasMaxLength(50);

        builder.Property(v => v.CommonCollocations)
            .HasMaxLength(1000);

        builder.Property(v => v.Antonyms)
            .HasMaxLength(500);

        builder.Property(v => v.KanjiComponents)
            .HasMaxLength(200);

        // Relationship with Card is configured in CardConfiguration
    }
}
