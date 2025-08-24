using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cdm.Data.Models.Configuration;

public class CampaignParticipantConfiguration : IEntityTypeConfiguration<CampaignParticipant>
{
    public void Configure(EntityTypeBuilder<CampaignParticipant> builder)
    {
        builder.HasKey(cp => cp.Id);

        // Index unique pour éviter les doublons utilisateur/campagne
        builder.HasIndex(cp => new { cp.CampaignId, cp.UserId })
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        builder.Property(cp => cp.Role)
            .HasConversion<int>();

        builder.Property(cp => cp.Permissions)
            .HasConversion<int>();

        // Relations
        builder.HasOne(cp => cp.Campaign)
            .WithMany()
            .HasForeignKey(cp => cp.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cp => cp.User)
            .WithMany()
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cp => cp.Invitation)
            .WithMany()
            .HasForeignKey(cp => cp.InvitationId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}