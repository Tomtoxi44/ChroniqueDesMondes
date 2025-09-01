using Cdm.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cdm.Data.Common.Models.Configuration;

/// <summary>
/// Configuration EF Core pour AEquipment (classe de base)
/// </summary>
public class AEquipmentConfiguration : IEntityTypeConfiguration<AEquipment>
{
    public void Configure(EntityTypeBuilder<AEquipment> builder)
    {
        builder.ToTable("Equipment");

        // Clé primaire
        builder.HasKey(e => e.Id);

        // Propriétés requises
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.GameType)
            .IsRequired()
            .HasConversion<string>(); // Stocké en string en base

        builder.Property(e => e.CreatedByUserId)
            .IsRequired();

        builder.Property(e => e.IsPublic)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Weight)
            .HasColumnType("decimal(10,3)"); // Précision pour le poids

        builder.Property(e => e.Value)
            .HasColumnType("decimal(10,2)"); // Précision pour la valeur

        // Propriétés optionnelles
        builder.Property(e => e.ImageUrl)
            .HasMaxLength(500);

        builder.Property(e => e.Tags)
            .HasMaxLength(1000)
            .HasColumnType("nvarchar(1000)"); // JSON array de tags

        // Relations
        builder.HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index pour les performances
        builder.HasIndex(e => e.GameType)
            .HasDatabaseName("IX_Equipment_GameType");

        builder.HasIndex(e => e.CreatedByUserId)
            .HasDatabaseName("IX_Equipment_CreatedByUserId");

        builder.HasIndex(e => new { e.GameType, e.IsPublic, e.IsActive })
            .HasDatabaseName("IX_Equipment_GameType_IsPublic_IsActive");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Equipment_Name");

        builder.HasIndex(e => e.Category)
            .HasDatabaseName("IX_Equipment_Category");

        builder.HasIndex(e => new { e.Category, e.GameType })
            .HasDatabaseName("IX_Equipment_Category_GameType");

        // Propriétés calculées ignorées (pas mappées en base)
        builder.Ignore(e => e.Source);
    }
}