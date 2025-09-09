using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cdm.Common.Enums;

namespace Cdm.Data.Common.Models.Combat;

/// <summary>
/// Action effectuée pendant un combat (attaque, sort, mouvement, etc.)
/// Enregistre tous les détails pour l'historique et les statistiques
/// </summary>
[Table("CombatActions")]
public class CombatAction
{
    public int Id { get; set; }

    /// <summary>
    /// Combat dans lequel l'action a été effectuée
    /// </summary>
    [Required]
    public int CombatId { get; set; }

    [ForeignKey(nameof(CombatId))]
    public virtual CombatSession Combat { get; set; } = null!;

    /// <summary>
    /// Participant qui effectue l'action
    /// </summary>
    [Required]
    public int ActorId { get; set; }

    [ForeignKey(nameof(ActorId))]
    public virtual CombatParticipant Actor { get; set; } = null!;

    /// <summary>
    /// Participant ciblé par l'action (optionnel)
    /// </summary>
    public int? TargetId { get; set; }

    [ForeignKey(nameof(TargetId))]
    public virtual CombatParticipant? Target { get; set; }

    /// <summary>
    /// Round du combat où l'action a eu lieu
    /// </summary>
    [Required]
    public int Round { get; set; }

    /// <summary>
    /// Numéro de l'action dans le tour (1, 2, 3...)
    /// Permet les actions multiples (Action Surge, etc.)
    /// </summary>
    public int ActionSequence { get; set; } = 1;

    /// <summary>
    /// Type d'action selon D&D 5e ou système générique
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ActionType { get; set; } = string.Empty; // attack, spell, move, dodge, help, dash, etc.

    /// <summary>
    /// Nom spécifique de l'action
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ActionName { get; set; } = string.Empty; // "Attaque à l'épée", "Boule de feu", "Esquive"

    /// <summary>
    /// Description de l'action pour l'historique
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Description { get; set; }

    /// <summary>
    /// ID de l'équipement utilisé (arme, objet magique)
    /// </summary>
    public int? EquipmentId { get; set; }

    /// <summary>
    /// ID du sort lancé
    /// </summary>
    public int? SpellId { get; set; }

    /// <summary>
    /// Niveau du sort utilisé (pour les sorts)
    /// </summary>
    public int? SpellLevel { get; set; }

    /// <summary>
    /// Résultat des jets de dés effectués
    /// Format JSON: {"attack": 15, "damage": [8, 3], "save": null}
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? DiceRolls { get; set; }

    /// <summary>
    /// Dégâts infligés (si applicable)
    /// </summary>
    public int? DamageDealt { get; set; }

    /// <summary>
    /// Type de dégâts infligés
    /// </summary>
    [MaxLength(50)]
    public string? DamageType { get; set; }

    /// <summary>
    /// Soins effectués (si applicable)
    /// </summary>
    public int? HealingDone { get; set; }

    /// <summary>
    /// Indique si l'action a réussi
    /// </summary>
    public bool? IsSuccess { get; set; }

    /// <summary>
    /// Indique si c'était un coup critique
    /// </summary>
    public bool IsCritical { get; set; } = false;

    /// <summary>
    /// Avantage ou désavantage sur le jet
    /// </summary>
    [MaxLength(20)]
    public string? AdvantageType { get; set; } // "advantage", "disadvantage", "normal"

    /// <summary>
    /// Sauvegarde demandée (DD et type)
    /// Format JSON: {"dc": 15, "ability": "Dexterity", "success": true}
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? SavingThrow { get; set; }

    /// <summary>
    /// Effets secondaires appliqués
    /// Format JSON: [{"target": "self", "effect": "rage", "duration": 10}]
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Effects { get; set; }

    /// <summary>
    /// Ressources consommées pour cette action
    /// Format JSON: {"spellSlot": 2, "ki": 1, "superiority": 1}
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? ResourcesUsed { get; set; }

    /// <summary>
    /// Position avant l'action (pour les mouvements)
    /// </summary>
    [MaxLength(100)]
    public string? PositionBefore { get; set; }

    /// <summary>
    /// Position après l'action
    /// </summary>
    [MaxLength(100)]
    public string? PositionAfter { get; set; }

    /// <summary>
    /// Distance de mouvement
    /// </summary>
    public int? MovementDistance { get; set; }

    /// <summary>
    /// Données spécifiques au type de jeu
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? GameSpecificData { get; set; }

    /// <summary>
    /// Notes du MJ ou du joueur sur l'action
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Durée d'exécution de l'action (pour stats de performance)
    /// </summary>
    public TimeSpan? ExecutionTime { get; set; }

    // Propriétés calculées
    public bool IsAttack => ActionType == "attack" || ActionType == "spell_attack";
    public bool IsSpell => ActionType == "spell" || ActionType == "spell_attack";
    public bool IsMovement => ActionType == "move" || ActionType == "dash";
    public bool HasDamage => DamageDealt.HasValue && DamageDealt.Value > 0;
    public bool HasHealing => HealingDone.HasValue && HealingDone.Value > 0;

    /// <summary>
    /// Génère un message pour l'historique du combat
    /// </summary>
    public string GenerateLogMessage()
    {
        var message = $"{Actor.Name}";

        switch (ActionType.ToLower())
        {
            case "attack":
                if (Target != null)
                {
                    message += $" attaque {Target.Name}";
                    if (IsSuccess == true)
                    {
                        message += IsCritical ? " avec un coup critique" : " et touche";
                        if (HasDamage)
                            message += $" pour {DamageDealt} dégâts {DamageType}";
                    }
                    else
                    {
                        message += " mais rate son attaque";
                    }
                }
                break;

            case "spell":
            case "spell_attack":
                message += $" lance {ActionName}";
                if (Target != null)
                    message += $" sur {Target.Name}";
                if (HasDamage)
                    message += $" infligeant {DamageDealt} dégâts {DamageType}";
                if (HasHealing)
                    message += $" soignant {HealingDone} points de vie";
                break;

            case "move":
                message += $" se déplace";
                if (MovementDistance.HasValue)
                    message += $" de {MovementDistance}m";
                break;

            case "dodge":
                message += " prend une position défensive";
                break;

            case "help":
                if (Target != null)
                    message += $" aide {Target.Name}";
                else
                    message += " utilise l'action Aider";
                break;

            default:
                message += $" utilise {ActionName}";
                break;
        }

        return message;
    }
}