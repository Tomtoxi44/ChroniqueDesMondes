using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cdm.Common.Enums;

namespace Cdm.Data.Models.Configuration;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        // Primary key
        builder.HasKey(c => c.Id);

        // Indexes for performance optimization
        builder.HasIndex(c => c.GameType)
               .HasDatabaseName("IX_Campaigns_GameType");

        builder.HasIndex(c => c.IsPublic)
               .HasDatabaseName("IX_Campaigns_IsPublic");

        builder.HasIndex(c => c.CreatedById)
               .HasDatabaseName("IX_Campaigns_CreatedById");

        builder.HasIndex(c => new { c.GameType, c.IsPublic })
               .HasDatabaseName("IX_Campaigns_GameType_IsPublic");

        // Foreign key relationships
        builder.HasOne(c => c.CreatedBy)
               .WithMany()
               .HasForeignKey(c => c.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

        // One-to-many with Chapters
        builder.HasMany(c => c.Chapters)
               .WithOne(ch => ch.Campaign)
               .HasForeignKey(ch => ch.CampaignId)
               .OnDelete(DeleteBehavior.Cascade);

        // Property configurations
        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.Description)
               .HasMaxLength(2000);

        builder.Property(c => c.GameType)
               .IsRequired()
               .HasConversion<int>() // Store as int in database
               .HasDefaultValue(GameType.Generic);

        builder.Property(c => c.IsPublic)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(c => c.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(c => c.CreatedDate)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");
    }
}