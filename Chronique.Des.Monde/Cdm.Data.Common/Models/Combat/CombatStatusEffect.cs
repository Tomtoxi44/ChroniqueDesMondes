using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cdm.Data.Common.Models.Combat;

/// <summary>
/// Effet de statut appliqué à un participant de combat
/// Gère les buffs, debuffs, conditions et effets temporaires
/// </summary>
[Table("CombatStatusEffects")]
public class CombatStatusEffect
{
    public int Id { get; set; }

    /// <summary>
    /// Participant affecté par l'effet
    /// </summary>
    [Required]
    public int ParticipantId { get; set; }

    [ForeignKey(nameof(ParticipantId))]
    public virtual CombatParticipant Participant { get; set; } = null!;

    /// <summary>
    /// Nom de l'effet
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type d'effet: "condition", "buff", "debuff", "spell_effect"
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string EffectType { get; set; } = string.Empty;

    /// <summary>
    /// Description de l'effet
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Description { get; set; }

    /// <summary>
    /// Source de l'effet (sort, capacité, environnement)
    /// </summary>
    [MaxLength(100)]
    public string? Source { get; set; }

    /// <summary>
    /// ID du participant qui a causé l'effet
    /// </summary>
    public int? SourceParticipantId { get; set; }

    [ForeignKey(nameof(SourceParticipantId))]
    public virtual CombatParticipant? SourceParticipant { get; set; }

    /// <summary>
    /// Durée restante en rounds (-1 = permanent jusqu'à dissipation)
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Durée initiale de l'effet
    /// </summary>
    public int InitialDuration { get; set; }

    /// <summary>
    /// Intensité ou niveau de l'effet
    /// </summary>
    public int Intensity { get; set; } = 1;

    /// <summary>
    /// Indique si l'effet est actif
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Modificateurs appliqués par l'effet
    /// Format JSON: {"armorClass": -2, "speed": 0, "disadvantage": ["attack"], "advantage": ["save_constitution"]}
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Modifiers { get; set; }

    /// <summary>
    /// Conditions D&D 5e standard
    /// "blinded", "charmed", "deafened", "frightened", "grappled", "incapacitated", 
    /// "invisible", "paralyzed", "petrified", "poisoned", "prone", "restrained", "stunned", "unconscious"
    /// </summary>
    [MaxLength(50)]
    public string? Condition { get; set; }

    /// <summary>
    /// Dégâts ou soins périodiques
    /// Format JSON: {"type": "poison", "amount": "1d4", "timing": "start_of_turn"}
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? PeriodicEffect { get; set; }

    /// <summary>
    /// Sauvegarde pour résister ou terminer l'effet
    /// Format JSON: {"ability": "Constitution", "dc": 15, "timing": "end_of_turn"}
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? SavingThrow { get; set; }

    /// <summary>
    /// Icône ou couleur pour l'affichage
    /// </summary>
    [MaxLength(50)]
    public string? Icon { get; set; }

    /// <summary>
    /// Indique si l'effet peut être dissipé
    /// </summary>
    public bool CanBeDispelled { get; set; } = true;

    /// <summary>
    /// Indique si l'effet se cumule avec lui-même
    /// </summary>
    public bool IsStackable { get; set; } = false;

    /// <summary>
    /// Round où l'effet a été appliqué
    /// </summary>
    public int AppliedAtRound { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RemovedAt { get; set; }

    // Propriétés calculées
    public bool IsExpired => Duration == 0;
    public bool IsPermanent => InitialDuration == -1;
    public bool IsCondition => !string.IsNullOrEmpty(Condition);

    /// <summary>
    /// Réduit la durée de l'effet de 1 round
    /// </summary>
    public void AdvanceRound()
    {
        if (Duration > 0)
        {
            Duration--;
            if (Duration == 0)
            {
                IsActive = false;
                RemovedAt = DateTime.UtcNow;
            }
        }
    }

    /// <summary>
    /// Supprime immédiatement l'effet
    /// </summary>
    public void Remove()
    {
        IsActive = false;
        Duration = 0;
        RemovedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Génère la description pour l'interface
    /// </summary>
    public string GetDisplayText()
    {
        var text = Name;
        
        if (Duration > 0)
            text += $" ({Duration} rounds)";
        else if (IsPermanent)
            text += " (permanent)";

        if (Intensity > 1)
            text += $" (niveau {Intensity})";

        return text;
    }

    /// <summary>
    /// Obtient la couleur d'affichage selon le type d'effet
    /// </summary>
    public string GetDisplayColor() => EffectType.ToLower() switch
    {
        "buff" => "#4CAF50",      // Vert
        "debuff" => "#F44336",    // Rouge
        "condition" => "#FF9800", // Orange
        "spell_effect" => "#2196F3", // Bleu
        _ => "#9E9E9E"           // Gris par défaut
    };
}