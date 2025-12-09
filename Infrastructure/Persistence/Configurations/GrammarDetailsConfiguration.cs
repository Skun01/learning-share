using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class GrammarDetailsConfiguration : IEntityTypeConfiguration<GrammarDetails>
{
    public void Configure(EntityTypeBuilder<GrammarDetails> builder)
    {
        builder.ToTable("GrammarDetails");
            
        builder.HasKey(gd => gd.CardId);

        builder.Property(gd => gd.Level)
            .HasMaxLength(10)
            .HasConversion<string>();
    }
}
