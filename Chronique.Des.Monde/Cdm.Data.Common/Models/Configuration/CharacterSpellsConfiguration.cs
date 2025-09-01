using Cdm.Data.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cdm.Data.Common.Models.Configuration;

/// <summary>
/// Configuration EF Core pour CharacterSpells (liaison Character ↔ Spell)
/// Version simplifiée sans FK pour éviter les problèmes de dépendances circulaires
/// </summary>
public class CharacterSpellsConfiguration : IEntityTypeConfiguration<CharacterSpells>
{
    public void Configure(EntityTypeBuilder<CharacterSpells> builder)
    {
        builder.ToTable("CharacterSpells");

        // Clé primaire
        builder.HasKey(cs => cs.Id);

        // Propriétés requises
        builder.Property(cs => cs.CharacterId)
            .IsRequired();

        builder.Property(cs => cs.SpellId)
            .IsRequired();

        builder.Property(cs => cs.LearnedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Propriétés optionnelles
        builder.Property(cs => cs.Notes)
            .HasMaxLength(500);

        builder.Property(cs => cs.CustomName)
            .HasMaxLength(100);

        // Relations - COMMENTÉES pour éviter les FK manquantes
        // builder.HasOne(cs => cs.Character)
        //     .WithMany()
        //     .HasForeignKey(cs => cs.CharacterId)
        //     .OnDelete(DeleteBehavior.Cascade);

        // builder.HasOne(cs => cs.Spell)
        //     .WithMany()
        //     .HasForeignKey(cs => cs.SpellId)
        //     .OnDelete(DeleteBehavior.Restrict);

        // Index pour les performances et contraintes
        builder.HasIndex(cs => cs.CharacterId)
            .HasDatabaseName("IX_CharacterSpells_CharacterId");

        builder.HasIndex(cs => cs.SpellId)
            .HasDatabaseName("IX_CharacterSpells_SpellId");

        // Contrainte d'unicité : un personnage ne peut avoir qu'une fois le même sort
        builder.HasIndex(cs => new { cs.CharacterId, cs.SpellId })
            .HasDatabaseName("IX_CharacterSpells_Character_Spell")
            .IsUnique();

        // Index pour les sorts préparés (optimisation pour D&D)
        builder.HasIndex(cs => new { cs.CharacterId, cs.IsPrepared })
            .HasDatabaseName("IX_CharacterSpells_Character_Prepared");
    }
}