using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronique.Des.Mondes.Data.Models.Configuration;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        // Primary key
        builder.HasKey(c => c.Id);

        // Indexes for performance
        builder.HasIndex(c => c.GameType)
               .HasDatabaseName("IX_Campaigns_GameType");

        builder.HasIndex(c => c.IsPublic)
               .HasDatabaseName("IX_Campaigns_IsPublic");

        builder.HasIndex(c => c.CreatedById)
               .HasDatabaseName("IX_Campaigns_CreatedById");

        builder.HasIndex(c => new { c.GameType, c.IsPublic })
               .HasDatabaseName("IX_Campaigns_GameType_IsPublic");

        // Foreign key relationship
        builder.HasOne(c => c.CreatedBy)
               .WithMany()
               .HasForeignKey(c => c.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

        // One-to-many relationship with Chapters
        builder.HasMany(c => c.Chapters)
               .WithOne(ch => ch.Campaign)
               .HasForeignKey(ch => ch.CampaignId)
               .OnDelete(DeleteBehavior.Cascade);

        // Constraints
        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.Description)
               .HasMaxLength(2000);

        builder.Property(c => c.GameType)
               .IsRequired();

        builder.Property(c => c.CreatedDate)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");
    }
}