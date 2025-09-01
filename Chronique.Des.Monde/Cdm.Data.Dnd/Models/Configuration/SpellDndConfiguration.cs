using Cdm.Data.Dnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cdm.Data.Dnd.Models.Configuration;

/// <summary>
/// Configuration EF Core pour SpellDnd
/// </summary>
public class SpellDndConfiguration : IEntityTypeConfiguration<SpellDnd>
{
    public void Configure(EntityTypeBuilder<SpellDnd> builder)
    {
        // Hérite de la configuration de base ASpell
        // Les propriétés de base sont configurées dans ASpellConfiguration

        // Configuration des propriétés spécifiques D&D
        builder.Property(s => s.School)
            .HasMaxLength(50);

        builder.Property(s => s.Level)
            .IsRequired();

        builder.Property(s => s.CastingTime)
            .HasMaxLength(100);

        builder.Property(s => s.Range)
            .HasMaxLength(100);

        builder.Property(s => s.Duration)
            .HasMaxLength(200);

        builder.Property(s => s.Components)
            .HasMaxLength(50);

        builder.Property(s => s.AttackRoll)
            .HasMaxLength(100);

        builder.Property(s => s.Damage)
            .HasMaxLength(100);

        builder.Property(s => s.SavingThrow)
            .HasMaxLength(50);

        builder.Property(s => s.MaterialComponent)
            .HasMaxLength(500);

        // Index spécifiques D&D
        builder.HasIndex(s => s.School)
            .HasDatabaseName("IX_Spells_School");

        builder.HasIndex(s => s.Level)
            .HasDatabaseName("IX_Spells_Level");

        builder.HasIndex(s => new { s.Level, s.School })
            .HasDatabaseName("IX_Spells_Level_School");
    }
}