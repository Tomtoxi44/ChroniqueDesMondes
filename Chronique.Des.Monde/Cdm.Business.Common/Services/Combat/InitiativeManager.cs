using Cdm.Business.Common.Interfaces.Combat;
using Cdm.Business.Common.Models.Combat;
using Cdm.Data.Common.Models.Combat;
using Microsoft.Extensions.Logging;

namespace Cdm.Business.Common.Services.Combat;

/// <summary>
/// Gestionnaire de l'initiative et de l'ordre des tours en combat
/// Implémente les règles D&D 5e pour l'initiative
/// </summary>
public class InitiativeManager : IInitiativeManager
{
    private readonly ILogger<InitiativeManager> _logger;

    public InitiativeManager(ILogger<InitiativeManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Calcule l'ordre d'initiative pour tous les participants
    /// </summary>
    public List<InitiativeOrderDto> CalculateTurnOrder(List<CombatParticipant> participants)
    {
        if (!participants.Any())
        {
            _logger.LogWarning("Aucun participant pour calculer l'ordre d'initiative");
            return new List<InitiativeOrderDto>();
        }

        // Filtrer les participants actifs avec initiative
        var activeParticipants = participants
            .Where(p => p.IsActive && p.Initiative.HasValue)
            .ToList();

        if (!activeParticipants.Any())
        {
            _logger.LogWarning("Aucun participant actif avec initiative lancée");
            return new List<InitiativeOrderDto>();
        }

        // Trier par initiative (décroissant), puis par départage (décroissant)
        var orderedParticipants = activeParticipants
            .OrderByDescending(p => p.Initiative!.Value)
            .ThenByDescending(p => p.InitiativeTiebreaker)
            .ThenBy(p => p.Name) // Départage final par ordre alphabétique
            .ToList();

        _logger.LogInformation("Ordre d'initiative calculé pour {Count} participants", orderedParticipants.Count);

        return orderedParticipants.Select((p, index) => new InitiativeOrderDto
        {
            ParticipantId = p.Id,
            Name = p.Name,
            ParticipantType = p.ParticipantType,
            Initiative = p.Initiative!.Value,
            IsActive = p.IsActive && p.CurrentHitPoints > 0,
            IsCurrent = index == 0, // Le premier est celui qui commence
            HealthPercentage = p.HealthPercentage,
            StatusEffects = ParseStatusEffects(p.StatusEffects)
        }).ToList();
    }

    /// <summary>
    /// Obtient le participant suivant dans l'ordre
    /// </summary>
    public CombatParticipant? GetNextParticipant(List<CombatParticipant> participants, int currentIndex)
    {
        var activeParticipants = participants
            .Where(p => p.IsActive && p.Initiative.HasValue && p.CurrentHitPoints > 0)
            .OrderByDescending(p => p.Initiative!.Value)
            .ThenByDescending(p => p.InitiativeTiebreaker)
            .ThenBy(p => p.Name)
            .ToList();

        if (!activeParticipants.Any())
        {
            _logger.LogWarning("Aucun participant actif pour le tour suivant");
            return null;
        }

        // Passer au participant suivant, en bouclant si nécessaire
        var nextIndex = (currentIndex + 1) % activeParticipants.Count;
        var nextParticipant = activeParticipants[nextIndex];

        _logger.LogInformation("Tour suivant: {ParticipantName} (index {Index})", 
            nextParticipant.Name, nextIndex);

        return nextParticipant;
    }

    /// <summary>
    /// Gère les égalités d'initiative selon les règles D&D 5e
    /// </summary>
    public List<CombatParticipant> ResolveTies(List<CombatParticipant> participants)
    {
        var groups = participants
            .Where(p => p.Initiative.HasValue)
            .GroupBy(p => p.Initiative!.Value)
            .ToList();

        var tiesFound = groups.Any(g => g.Count() > 1);
        
        if (tiesFound)
        {
            _logger.LogInformation("Égalités d'initiative détectées, résolution par modificateur de Dextérité");
            
            foreach (var group in groups.Where(g => g.Count() > 1))
            {
                var tiedParticipants = group.ToList();
                _logger.LogInformation("Initiative {Value}: {Participants}", 
                    group.Key, string.Join(", ", tiedParticipants.Select(p => p.Name)));
            }
        }

        // Retourne la liste triée avec départage par InitiativeTiebreaker
        return participants
            .Where(p => p.Initiative.HasValue)
            .OrderByDescending(p => p.Initiative!.Value)
            .ThenByDescending(p => p.InitiativeTiebreaker) // Modificateur de Dextérité
            .ThenBy(p => p.ParticipantType == "player" ? 0 : 1) // Joueurs en premier en cas d'égalité parfaite
            .ThenBy(p => p.Name)
            .ToList();
    }

    /// <summary>
    /// Insère un nouveau participant dans l'ordre existant
    /// </summary>
    public int InsertParticipantInOrder(List<CombatParticipant> participants, CombatParticipant newParticipant)
    {
        if (!newParticipant.Initiative.HasValue)
        {
            _logger.LogError("Impossible d'insérer un participant sans initiative");
            return -1;
        }

        var orderedParticipants = ResolveTies(participants);
        
        // Trouver la position d'insertion
        var insertIndex = 0;
        for (int i = 0; i < orderedParticipants.Count; i++)
        {
            var current = orderedParticipants[i];
            
            // Si le nouveau participant a une initiative plus élevée
            if (newParticipant.Initiative > current.Initiative)
            {
                insertIndex = i;
                break;
            }
            
            // Si même initiative, départager par tiebreaker
            if (newParticipant.Initiative == current.Initiative)
            {
                if (newParticipant.InitiativeTiebreaker > current.InitiativeTiebreaker)
                {
                    insertIndex = i;
                    break;
                }
                
                // Si même tiebreaker, les joueurs passent avant les PNJ
                if (newParticipant.InitiativeTiebreaker == current.InitiativeTiebreaker)
                {
                    if (newParticipant.ParticipantType == "player" && current.ParticipantType == "npc")
                    {
                        insertIndex = i;
                        break;
                    }
                }
            }
            
            insertIndex = i + 1;
        }

        _logger.LogInformation("Nouveau participant {Name} inséré à la position {Position} avec initiative {Initiative}", 
            newParticipant.Name, insertIndex, newParticipant.Initiative);

        return insertIndex;
    }

    /// <summary>
    /// Vérifie si tous les participants ont lancé leur initiative
    /// </summary>
    public bool AllInitiativesRolled(List<CombatParticipant> participants)
    {
        var activeParticipants = participants.Where(p => p.IsActive).ToList();
        var rolledCount = activeParticipants.Count(p => p.Initiative.HasValue);
        
        _logger.LogDebug("Initiative: {Rolled}/{Total} participants ont lancé", 
            rolledCount, activeParticipants.Count);
        
        return rolledCount == activeParticipants.Count && activeParticipants.Any();
    }

    /// <summary>
    /// Génère un rapport de l'ordre d'initiative
    /// </summary>
    public string GenerateInitiativeReport(List<CombatParticipant> participants)
    {
        var orderedParticipants = ResolveTies(participants);
        var report = new List<string> { "🎯 ORDRE D'INITIATIVE:" };
        
        for (int i = 0; i < orderedParticipants.Count; i++)
        {
            var p = orderedParticipants[i];
            var status = p.CurrentHitPoints <= 0 ? " [KO]" : "";
            var type = p.ParticipantType == "player" ? "👤" : "🤖";
            
            report.Add($"{i + 1}. {type} {p.Name} - Initiative {p.Initiative} (DEX+{p.InitiativeTiebreaker}){status}");
        }
        
        var finalReport = string.Join("\n", report);
        _logger.LogInformation("Rapport d'initiative généré:\n{Report}", finalReport);
        
        return finalReport;
    }

    /// <summary>
    /// Recalcule l'ordre après qu'un participant soit éliminé
    /// </summary>
    public List<InitiativeOrderDto> RecalculateAfterElimination(List<CombatParticipant> participants, int eliminatedParticipantId)
    {
        _logger.LogInformation("Recalcul de l'initiative après élimination du participant {Id}", eliminatedParticipantId);
        
        var activeParticipants = participants
            .Where(p => p.IsActive && p.CurrentHitPoints > 0 && p.Id != eliminatedParticipantId)
            .ToList();
        
        return CalculateTurnOrder(activeParticipants);
    }

    /// <summary>
    /// Parse les effets de statut depuis le JSON
    /// </summary>
    private static List<StatusEffectDto> ParseStatusEffects(string? statusEffectsJson)
    {
        if (string.IsNullOrEmpty(statusEffectsJson))
            return new List<StatusEffectDto>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<StatusEffectDto>>(statusEffectsJson) 
                   ?? new List<StatusEffectDto>();
        }
        catch
        {
            return new List<StatusEffectDto>();
        }
    }

    /// <summary>
    /// Obtient le délai moyen entre les tours pour un participant
    /// </summary>
    public TimeSpan EstimateTurnInterval(List<CombatParticipant> participants, TimeSpan? averageTurnDuration = null)
    {
        var activeCount = participants.Count(p => p.IsActive && p.CurrentHitPoints > 0);
        var turnDuration = averageTurnDuration ?? TimeSpan.FromMinutes(2); // 2 minutes par défaut
        
        return TimeSpan.FromTicks(turnDuration.Ticks * activeCount);
    }
}