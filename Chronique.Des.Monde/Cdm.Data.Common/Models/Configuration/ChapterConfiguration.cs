using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cdm.Data.Models.Configuration;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        // Primary key
        builder.HasKey(c => c.Id);

        // Indexes for performance
        builder.HasIndex(c => c.CampaignId)
               .HasDatabaseName("IX_Chapters_CampaignId");

        builder.HasIndex(c => c.IsActive)
               .HasDatabaseName("IX_Chapters_IsActive");

        builder.HasIndex(c => new { c.CampaignId, c.Order })
               .HasDatabaseName("IX_Chapters_CampaignId_Order")
               .IsUnique();

        // Foreign key relationships
        builder.HasOne(c => c.Campaign)
               .WithMany(camp => camp.Chapters)
               .HasForeignKey(c => c.CampaignId)
               .OnDelete(DeleteBehavior.Cascade);

        // One-to-many with ContentBlocks
        builder.HasMany(c => c.ContentBlocks)
               .WithOne(cb => cb.Chapter)
               .HasForeignKey(cb => cb.ChapterId)
               .OnDelete(DeleteBehavior.Cascade);

        // One-to-many with Characters (NPCs)
        builder.HasMany(c => c.Characters)
               .WithOne(ch => ch.Chapter)
               .HasForeignKey(ch => ch.ChapterId)
               .OnDelete(DeleteBehavior.SetNull);

        // Property configurations
        builder.Property(c => c.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(c => c.Order)
               .IsRequired();

        builder.Property(c => c.IsActive)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(c => c.CreatedDate)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");
    }
}