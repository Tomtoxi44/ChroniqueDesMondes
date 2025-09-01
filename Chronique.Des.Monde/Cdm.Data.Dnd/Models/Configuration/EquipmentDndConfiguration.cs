using Cdm.Data.Dnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cdm.Data.Dnd.Models.Configuration;

/// <summary>
/// Configuration EF Core pour EquipmentDnd
/// </summary>
public class EquipmentDndConfiguration : IEntityTypeConfiguration<EquipmentDnd>
{
    public void Configure(EntityTypeBuilder<EquipmentDnd> builder)
    {
        // Hérite de la configuration de base AEquipment
        // Les propriétés de base sont configurées dans AEquipmentConfiguration

        // Configuration des propriétés spécifiques D&D
        builder.Property(e => e.WeaponType)
            .HasMaxLength(50);

        builder.Property(e => e.Damage)
            .HasMaxLength(100);

        builder.Property(e => e.DamageType)
            .HasMaxLength(50);

        builder.Property(e => e.Properties)
            .HasMaxLength(100);

        builder.Property(e => e.Rarity)
            .HasMaxLength(50)
            .HasDefaultValue("Common");

        builder.Property(e => e.MagicalProperties)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.Effect)
            .HasMaxLength(200);

        builder.Property(e => e.MaxDexBonus)
            .HasDefaultValue(-1); // -1 = pas de limite

        // Index spécifiques D&D
        builder.HasIndex(e => e.WeaponType)
            .HasDatabaseName("IX_Equipment_WeaponType");

        builder.HasIndex(e => e.DamageType)
            .HasDatabaseName("IX_Equipment_DamageType");

        builder.HasIndex(e => e.Rarity)
            .HasDatabaseName("IX_Equipment_Rarity");

        builder.HasIndex(e => new { e.IsWeapon, e.IsArmor })
            .HasDatabaseName("IX_Equipment_Type_Flags");

        builder.HasIndex(e => e.ArmorClass)
            .HasDatabaseName("IX_Equipment_ArmorClass");

        builder.HasIndex(e => e.AttackBonus)
            .HasDatabaseName("IX_Equipment_AttackBonus");
    }
}