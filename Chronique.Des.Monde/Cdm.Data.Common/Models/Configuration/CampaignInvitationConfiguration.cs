using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cdm.Data.Models.Configuration;

public class CampaignInvitationConfiguration : IEntityTypeConfiguration<CampaignInvitation>
{
    public void Configure(EntityTypeBuilder<CampaignInvitation> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ci => ci.Token)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(ci => ci.Token)
            .IsUnique();

        builder.HasIndex(ci => new { ci.CampaignId, ci.Email })
            .IsUnique()
            .HasFilter("[Status] = 0"); // Unique seulement pour Pending

        builder.Property(ci => ci.Status)
            .HasConversion<int>();

        builder.Property(ci => ci.Message)
            .HasMaxLength(500);

        // Relations
        builder.HasOne(ci => ci.Campaign)
            .WithMany()
            .HasForeignKey(ci => ci.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ci => ci.User)
            .WithMany()
            .HasForeignKey(ci => ci.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(ci => ci.InvitedBy)
            .WithMany()
            .HasForeignKey(ci => ci.InvitedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}