using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.ToTable("UserSettings");

            builder.HasKey(us => us.UserId);

            builder.Property(us => us.EnableGhostMode)
                .HasDefaultValue(true);

            builder.Property(us => us.DailyGoal)
                .HasDefaultValue(10);

            // Cấu hình Enum -> String
            builder.Property(us => us.UiLanguage)
                .HasMaxLength(5)
                .HasDefaultValue(UiLanguage.Vi)
                .HasConversion<string>();
    }
}
