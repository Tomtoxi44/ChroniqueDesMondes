using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cdm.Common.Enums;
using Cdm.Data.Models;

namespace Cdm.Data.Common.Models.Combat;

/// <summary>
/// Session de combat active avec gestion des tours et état en temps réel
/// Compatible avec tous les GameTypes (Generic, DnD, etc.)
/// </summary>
[Table("CombatSessions")]
public class CombatSession
{
    public int Id { get; set; }

    /// <summary>
    /// ID unique de la session de jeu parente
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Chapitre dans lequel se déroule le combat
    /// </summary>
    [Required]
    public int ChapterId { get; set; }

    [ForeignKey(nameof(ChapterId))]
    public virtual Chapter Chapter { get; set; } = null!;

    /// <summary>
    /// Nom du combat pour identification
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type de jeu pour adapter les règles
    /// </summary>
    [Required]
    public GameType GameType { get; set; }

    /// <summary>
    /// Statut actuel du combat
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "initiative"; // initiative, active, paused, ended

    /// <summary>
    /// Round actuel du combat (commence à 1)
    /// </summary>
    public int CurrentRound { get; set; } = 1;

    /// <summary>
    /// Index du participant actuel dans l'ordre d'initiative
    /// </summary>
    public int CurrentTurnIndex { get; set; } = 0;

    /// <summary>
    /// Ordre d'initiative calculé et stocké en JSON
    /// Format: [{"participantId": 1, "initiative": 18, "name": "Lyralei"}, ...]
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? TurnOrder { get; set; }

    /// <summary>
    /// Paramètres spécifiques au combat (limite de temps, règles maison, etc.)
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Settings { get; set; }

    /// <summary>
    /// Environnement du combat (terrain, éclairage, obstacles)
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Environment { get; set; }

    /// <summary>
    /// Historique des actions du combat
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? ActionLog { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }

    /// <summary>
    /// Durée estimée du tour actuel (pour les notifications)
    /// </summary>
    public TimeSpan? TurnTimeLimit { get; set; }

    /// <summary>
    /// Heure de début du tour actuel
    /// </summary>
    public DateTime? CurrentTurnStarted { get; set; }

    /// <summary>
    /// Participants au combat
    /// </summary>
    public virtual ICollection<CombatParticipant> Participants { get; set; } = new List<CombatParticipant>();

    /// <summary>
    /// Actions effectuées pendant le combat
    /// </summary>
    public virtual ICollection<CombatAction> Actions { get; set; } = new List<CombatAction>();

    // Méthodes utilitaires
    public bool IsActive => Status == "active";
    public bool IsEnded => Status == "ended";
    public bool HasStarted => Status != "initiative";

    /// <summary>
    /// Obtient le participant dont c'est actuellement le tour
    /// </summary>
    public CombatParticipant? GetCurrentParticipant()
    {
        var orderedParticipants = Participants
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.Initiative)
            .ThenBy(p => p.InitiativeTiebreaker)
            .ToList();

        if (!orderedParticipants.Any() || CurrentTurnIndex >= orderedParticipants.Count)
            return null;

        return orderedParticipants[CurrentTurnIndex];
    }

    /// <summary>
    /// Vérifie si tous les participants ont lancé leur initiative
    /// </summary>
    public bool AllInitiativesRolled()
    {
        return Participants.All(p => p.Initiative.HasValue);
    }

    /// <summary>
    /// Calcule si le combat est terminé (un seul camp reste)
    /// </summary>
    public bool IsCombatOver()
    {
        var activeParticipants = Participants.Where(p => p.IsActive && p.CurrentHitPoints > 0).ToList();
        
        if (activeParticipants.Count <= 1)
            return true;

        // Vérifier s'il ne reste qu'un seul type de participant (joueurs vs PNJ)
        var playerCount = activeParticipants.Count(p => p.ParticipantType == "player");
        var npcCount = activeParticipants.Count(p => p.ParticipantType == "npc");

        return playerCount == 0 || npcCount == 0;
    }
}