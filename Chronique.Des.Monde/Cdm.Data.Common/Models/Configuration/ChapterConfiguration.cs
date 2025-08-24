using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronique.Des.Mondes.Data.Models.Configuration;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        // Primary key
        builder.HasKey(ch => ch.Id);

        // Indexes for performance
        builder.HasIndex(ch => ch.CampaignId)
               .HasDatabaseName("IX_Chapters_CampaignId");

        builder.HasIndex(ch => new { ch.CampaignId, ch.Order })
               .HasDatabaseName("IX_Chapters_CampaignId_Order")
               .IsUnique();

        builder.HasIndex(ch => ch.IsActive)
               .HasDatabaseName("IX_Chapters_IsActive");

        // Foreign key relationship
        builder.HasOne(ch => ch.Campaign)
               .WithMany(c => c.Chapters)
               .HasForeignKey(ch => ch.CampaignId)
               .OnDelete(DeleteBehavior.Cascade);

        // Constraints
        builder.Property(ch => ch.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(ch => ch.Order)
               .IsRequired();

        builder.Property(ch => ch.CreatedDate)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");
    }
}