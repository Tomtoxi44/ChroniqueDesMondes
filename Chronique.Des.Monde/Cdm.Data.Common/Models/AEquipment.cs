namespace Cdm.Data.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Cdm.Common.Enums;

/// <summary>
/// Classe de base abstraite pour tous les équipements, suivant le pattern ACharacter et ASpell
/// </summary>
public abstract class AEquipment
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Required]
    public GameType GameType { get; set; } = GameType.Generic;

    [Required]
    public int CreatedByUserId { get; set; } = 0; // 0 = Administrateur (équipements officiels)

    [ForeignKey(nameof(CreatedByUserId))]
    public virtual User? CreatedBy { get; set; }

    public bool IsPublic { get; set; } = true; // true pour officiels, false pour privés

    [Column(TypeName = "nvarchar(1000)")]
    public string? Tags { get; set; } // JSON array de tags pour recherche

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty; // Ex: "Weapon", "Armor", "Tool", "Consumable"

    public decimal Weight { get; set; } = 0; // Poids en kg

    public decimal Value { get; set; } = 0; // Valeur en pièces d'or (ou équivalent)

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Propriétés calculées (non mappées en base)
    [NotMapped]
    public EquipmentSource Source => this.CreatedByUserId == 0 ? EquipmentSource.Official : EquipmentSource.Private;

    // Méthodes utilitaires pour les tags
    public List<string> GetTags()
    {
        if (string.IsNullOrEmpty(this.Tags))
            return new List<string>();

        try
        {
            return JsonSerializer.Deserialize<List<string>>(this.Tags) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public void SetTags(List<string> tags)
    {
        this.Tags = tags?.Count > 0 ? JsonSerializer.Serialize(tags) : null;
    }
}

/// <summary>
/// Source de l'équipement - calculée selon CreatedByUserId
/// </summary>
public enum EquipmentSource
{
    Official, // Créé par admin (CreatedByUserId = 0)
    Private   // Créé par utilisateur (CreatedByUserId > 0)
}