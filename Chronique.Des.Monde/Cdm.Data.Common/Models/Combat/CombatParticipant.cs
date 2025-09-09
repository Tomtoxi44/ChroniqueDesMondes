using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cdm.Data.Models;

namespace Cdm.Data.Common.Models.Combat;

/// <summary>
/// Participant dans un combat (joueur ou PNJ)
/// Gère les statistiques de combat et l'état en temps réel
/// </summary>
[Table("CombatParticipants")]
public class CombatParticipant
{
    public int Id { get; set; }

    /// <summary>
    /// Combat auquel appartient ce participant
    /// </summary>
    [Required]
    public int CombatId { get; set; }

    [ForeignKey(nameof(CombatId))]
    public virtual CombatSession Combat { get; set; } = null!;

    /// <summary>
    /// Type de participant: "player" ou "npc"
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string ParticipantType { get; set; } = string.Empty;

    /// <summary>
    /// ID du personnage joueur (si ParticipantType = "player")
    /// Note: La relation vers CharacterDnd sera configurée dans DndDbContext
    /// </summary>
    public int? CharacterId { get; set; }

    /// <summary>
    /// ID du PNJ (si ParticipantType = "npc")
    /// </summary>
    public int? NpcId { get; set; }

    // Note: Relations vers Character/NPC seront configurées dans les DbContext spécifiques

    /// <summary>
    /// Nom affiché du participant (calculé depuis Character/NPC ou surchargé)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Initiative lancée (1d20 + modificateur)
    /// Null si pas encore lancée
    /// </summary>
    public int? Initiative { get; set; }

    /// <summary>
    /// Valeur pour départager les égalités d'initiative
    /// Généralement le modificateur de Dextérité
    /// </summary>
    public int InitiativeTiebreaker { get; set; } = 0;

    /// <summary>
    /// Points de vie actuels
    /// </summary>
    [Required]
    public int CurrentHitPoints { get; set; }

    /// <summary>
    /// Points de vie maximum
    /// </summary>
    [Required]
    public int MaxHitPoints { get; set; }

    /// <summary>
    /// Points de vie temporaires
    /// </summary>
    public int TemporaryHitPoints { get; set; } = 0;

    /// <summary>
    /// Classe d'armure actuelle
    /// </summary>
    [Required]
    public int ArmorClass { get; set; }

    /// <summary>
    /// Vitesse de déplacement (en mètres pour D&D)
    /// </summary>
    public int Speed { get; set; } = 9; // 30 pieds = 9 mètres par défaut

    /// <summary>
    /// Indique si le participant est actif dans le combat
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indique si le participant est conscient
    /// </summary>
    public bool IsConscious { get; set; } = true;

    /// <summary>
    /// Position sur le champ de bataille (coordonnées ou description)
    /// </summary>
    [MaxLength(100)]
    public string? Position { get; set; }

    /// <summary>
    /// Effets de statut actifs (JSON)
    /// Format: [{"name": "Empoisonné", "duration": 3, "source": "Spider bite"}, ...]
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? StatusEffects { get; set; }

    /// <summary>
    /// Ressources consommables actuelles (sorts, capacités spéciales)
    /// Format JSON spécifique au GameType
    /// D&D: {"spellSlots": {"1": 2, "2": 1}, "ki": 3, "rages": 1}
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Resources { get; set; }

    /// <summary>
    /// Actions disponibles ce tour
    /// Format: ["action", "move", "bonus_action", "reaction"]
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? AvailableActions { get; set; }

    /// <summary>
    /// Données spécifiques au type de jeu (JSON)
    /// D&D: caractéristiques, sauvegardes, compétences
    /// Generic: stats personnalisées
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? GameSpecificData { get; set; }

    /// <summary>
    /// Heure du dernier tour joué
    /// </summary>
    public DateTime? LastTurnAt { get; set; }

    /// <summary>
    /// Nombre de tours joués dans ce combat
    /// </summary>
    public int TurnsPlayed { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Actions effectuées par ce participant
    /// </summary>
    public virtual ICollection<CombatAction> Actions { get; set; } = new List<CombatAction>();

    // Propriétés calculées
    public bool IsAlive => CurrentHitPoints > 0;
    public bool IsUnconscious => CurrentHitPoints <= 0 && CurrentHitPoints > -MaxHitPoints;
    public bool IsDead => CurrentHitPoints <= -MaxHitPoints;
    public bool IsPlayer => ParticipantType == "player";
    public bool IsNpc => ParticipantType == "npc";

    /// <summary>
    /// Calcule le pourcentage de santé restante
    /// </summary>
    public int HealthPercentage => MaxHitPoints > 0 ? 
        Math.Max(0, Math.Min(100, (CurrentHitPoints * 100) / MaxHitPoints)) : 0;

    /// <summary>
    /// Obtient la description de l'état de santé
    /// </summary>
    public string HealthStatus => HealthPercentage switch
    {
        <= 0 => "Inconscient",
        <= 25 => "Gravement blessé",
        <= 50 => "Blessé",
        <= 75 => "Légèrement blessé",
        _ => "En bonne santé"
    };

    /// <summary>
    /// Applique des dégâts au participant
    /// </summary>
    public void TakeDamage(int damage)
    {
        // D'abord absorber les PV temporaires
        if (TemporaryHitPoints > 0)
        {
            var tempDamage = Math.Min(TemporaryHitPoints, damage);
            TemporaryHitPoints -= tempDamage;
            damage -= tempDamage;
        }

        // Puis appliquer aux PV normaux
        CurrentHitPoints = Math.Max(-MaxHitPoints, CurrentHitPoints - damage);

        // Mettre à jour l'état de conscience
        IsConscious = CurrentHitPoints > 0;
        if (!IsConscious)
        {
            IsActive = false; // Les personnages inconscients ne peuvent plus agir
        }
    }

    /// <summary>
    /// Soigne le participant
    /// </summary>
    public void Heal(int healing)
    {
        CurrentHitPoints = Math.Min(MaxHitPoints, CurrentHitPoints + healing);
        
        // Si le personnage était inconscient et retrouve des PV
        if (CurrentHitPoints > 0 && !IsConscious)
        {
            IsConscious = true;
            IsActive = true;
        }
    }

    /// <summary>
    /// Ajoute des points de vie temporaires
    /// </summary>
    public void AddTemporaryHitPoints(int tempHp)
    {
        // Les PV temporaires ne se cumulent pas, on prend le maximum
        TemporaryHitPoints = Math.Max(TemporaryHitPoints, tempHp);
    }
}