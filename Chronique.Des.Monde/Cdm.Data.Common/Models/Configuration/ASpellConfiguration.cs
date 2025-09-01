using Cdm.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cdm.Data.Common.Models.Configuration;

/// <summary>
/// Configuration EF Core pour ASpell (classe de base)
/// </summary>
public class ASpellConfiguration : IEntityTypeConfiguration<ASpell>
{
    public void Configure(EntityTypeBuilder<ASpell> builder)
    {
        builder.ToTable("Spells");

        // Clé primaire
        builder.HasKey(s => s.Id);

        // Propriétés requises
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.GameType)
            .IsRequired()
            .HasConversion<string>(); // Stocké en string en base

        builder.Property(s => s.CreatedByUserId)
            .IsRequired();

        builder.Property(s => s.IsPublic)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Propriétés optionnelles
        builder.Property(s => s.ImageUrl)
            .HasMaxLength(500);

        builder.Property(s => s.Tags)
            .HasMaxLength(1000)
            .HasColumnType("nvarchar(1000)"); // JSON array de tags

        // Relations
        builder.HasOne(s => s.CreatedBy)
            .WithMany()
            .HasForeignKey(s => s.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index pour les performances
        builder.HasIndex(s => s.GameType)
            .HasDatabaseName("IX_Spells_GameType");

        builder.HasIndex(s => s.CreatedByUserId)
            .HasDatabaseName("IX_Spells_CreatedByUserId");

        builder.HasIndex(s => new { s.GameType, s.IsPublic, s.IsActive })
            .HasDatabaseName("IX_Spells_GameType_IsPublic_IsActive");

        builder.HasIndex(s => s.Name)
            .HasDatabaseName("IX_Spells_Name");

        // Propriétés calculées ignorées (pas mappées en base)
        builder.Ignore(s => s.Source);
    }
}