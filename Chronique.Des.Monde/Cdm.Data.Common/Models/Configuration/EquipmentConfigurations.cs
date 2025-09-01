using Cdm.Data.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cdm.Data.Common.Models.Configuration;

/// <summary>
/// Configuration EF Core pour EquipmentOffer
/// </summary>
public class EquipmentOfferConfiguration : IEntityTypeConfiguration<EquipmentOffer>
{
    public void Configure(EntityTypeBuilder<EquipmentOffer> builder)
    {
        builder.ToTable("EquipmentOffers");

        // Clé primaire
        builder.HasKey(o => o.Id);

        // Propriétés requises
        builder.Property(o => o.CampaignId)
            .IsRequired();

        builder.Property(o => o.GameMasterId)
            .IsRequired();

        builder.Property(o => o.TargetPlayerId)
            .IsRequired();

        builder.Property(o => o.EquipmentId)
            .IsRequired();

        builder.Property(o => o.Quantity)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(OfferStatus.Pending);

        builder.Property(o => o.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Propriétés optionnelles
        builder.Property(o => o.Message)
            .HasMaxLength(500);

        builder.Property(o => o.ResponseMessage)
            .HasMaxLength(500);

        // Relations
        builder.HasOne(o => o.Campaign)
            .WithMany()
            .HasForeignKey(o => o.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.GameMaster)
            .WithMany()
            .HasForeignKey(o => o.GameMasterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.TargetPlayer)
            .WithMany()
            .HasForeignKey(o => o.TargetPlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Equipment)
            .WithMany()
            .HasForeignKey(o => o.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index pour les performances
        builder.HasIndex(o => o.CampaignId)
            .HasDatabaseName("IX_EquipmentOffers_CampaignId");

        builder.HasIndex(o => o.TargetPlayerId)
            .HasDatabaseName("IX_EquipmentOffers_TargetPlayerId");

        builder.HasIndex(o => o.Status)
            .HasDatabaseName("IX_EquipmentOffers_Status");

        builder.HasIndex(o => new { o.CampaignId, o.TargetPlayerId, o.Status })
            .HasDatabaseName("IX_EquipmentOffers_Campaign_Player_Status");
    }
}

/// <summary>
/// Configuration EF Core pour EquipmentTrade
/// </summary>
public class EquipmentTradeConfiguration : IEntityTypeConfiguration<EquipmentTrade>
{
    public void Configure(EntityTypeBuilder<EquipmentTrade> builder)
    {
        builder.ToTable("EquipmentTrades");

        // Clé primaire
        builder.HasKey(t => t.Id);

        // Propriétés requises
        builder.Property(t => t.CampaignId)
            .IsRequired();

        builder.Property(t => t.FromPlayerId)
            .IsRequired();

        builder.Property(t => t.ToPlayerId)
            .IsRequired();

        builder.Property(t => t.EquipmentId)
            .IsRequired();

        builder.Property(t => t.Quantity)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(TradeStatus.Pending);

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Propriétés optionnelles
        builder.Property(t => t.Message)
            .HasMaxLength(500);

        // Relations
        builder.HasOne(t => t.Campaign)
            .WithMany()
            .HasForeignKey(t => t.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.FromPlayer)
            .WithMany()
            .HasForeignKey(t => t.FromPlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.ToPlayer)
            .WithMany()
            .HasForeignKey(t => t.ToPlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Equipment)
            .WithMany()
            .HasForeignKey(t => t.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index pour les performances
        builder.HasIndex(t => t.CampaignId)
            .HasDatabaseName("IX_EquipmentTrades_CampaignId");

        builder.HasIndex(t => t.FromPlayerId)
            .HasDatabaseName("IX_EquipmentTrades_FromPlayerId");

        builder.HasIndex(t => t.ToPlayerId)
            .HasDatabaseName("IX_EquipmentTrades_ToPlayerId");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_EquipmentTrades_Status");

        builder.HasIndex(t => new { t.CampaignId, t.Status })
            .HasDatabaseName("IX_EquipmentTrades_Campaign_Status");
    }
}

/// <summary>
/// Configuration EF Core pour CharacterInventory
/// </summary>
public class CharacterInventoryConfiguration : IEntityTypeConfiguration<CharacterInventory>
{
    public void Configure(EntityTypeBuilder<CharacterInventory> builder)
    {
        builder.ToTable("CharacterInventory");

        // Clé primaire
        builder.HasKey(i => i.Id);

        // Propriétés requises
        builder.Property(i => i.CharacterId)
            .IsRequired();

        builder.Property(i => i.EquipmentId)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.ObtainedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Propriétés optionnelles
        builder.Property(i => i.Notes)
            .HasMaxLength(500);

        // Relations
        builder.HasOne(i => i.Character)
            .WithMany()
            .HasForeignKey(i => i.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Equipment)
            .WithMany()
            .HasForeignKey(i => i.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index pour les performances
        builder.HasIndex(i => i.CharacterId)
            .HasDatabaseName("IX_CharacterInventory_CharacterId");

        builder.HasIndex(i => i.EquipmentId)
            .HasDatabaseName("IX_CharacterInventory_EquipmentId");

        builder.HasIndex(i => new { i.CharacterId, i.EquipmentId })
            .HasDatabaseName("IX_CharacterInventory_Character_Equipment")
            .IsUnique();

        builder.HasIndex(i => new { i.CharacterId, i.IsEquipped })
            .HasDatabaseName("IX_CharacterInventory_Character_Equipped");
    }
}