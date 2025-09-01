using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cdm.Data.Models;

namespace Cdm.Data.Common.Models;

/// <summary>
/// Liaison many-to-many entre un personnage et ses sorts appris
/// Version simplifiée sans navigation properties pour éviter les dépendances
/// </summary>
public class CharacterSpells
{
    public int Id { get; set; }

    [Required]
    public int CharacterId { get; set; }

    // Navigation property commentée temporairement
    // [ForeignKey(nameof(CharacterId))]
    // public virtual ACharacter Character { get; set; } = null!;

    [Required]
    public int SpellId { get; set; }

    // Navigation property commentée temporairement
    // [ForeignKey(nameof(SpellId))]
    // public virtual ASpell Spell { get; set; } = null!;

    public DateTime LearnedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Notes { get; set; } // Notes personnelles du joueur sur ce sort

    // Propriétés spécifiques aux sorts préparés (pour D&D par exemple)
    public bool IsPrepared { get; set; } = false; // Si le sort est préparé (D&D)

    public int? SpellSlotLevel { get; set; } // Niveau d'emplacement utilisé pour lancer ce sort

    [MaxLength(100)]
    public string? CustomName { get; set; } // Nom personnalisé pour ce sort (si le joueur veut le renommer)
}