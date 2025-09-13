using Cdm.Web.Models.Api;
using Cdm.Web.Services.Api;

namespace Cdm.Web.Services.Combat;

/// <summary>
/// Modèles pour le système de combat
/// </summary>
public record CombatStateDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Status { get; init; } = "";
    public int CurrentRound { get; init; }
    public CombatParticipantDto? CurrentParticipant { get; init; }
    public List<CombatParticipantDto> Participants { get; init; } = new();
    public bool IsPaused { get; init; }
}

public record CombatParticipantDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string ParticipantType { get; init; } = "";
    public int Initiative { get; init; }
    public int HitPoints { get; init; }
    public int MaxHitPoints { get; init; }
    public int ArmorClass { get; init; }
    public List<StatusEffectDto> StatusEffects { get; init; } = new();
}

public record StatusEffectDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public int Duration { get; init; }
    public string Type { get; init; } = "";
}

public record StartCombatRequest
{
    public string Name { get; init; } = "";
    public List<CombatParticipantDto> Participants { get; init; } = new();
}

public record CombatActionRequest
{
    public int ActorId { get; init; }
    public string ActionType { get; init; } = "";
    public int? TargetId { get; init; }
    public string? SpellName { get; init; }
    public string? WeaponName { get; init; }
}

public record CombatActionResult
{
    public bool Success { get; init; }
    public string ActionDescription { get; init; } = "";
    public int? Damage { get; init; }
    public string? ErrorMessage { get; init; }
    public CombatStateDto? UpdatedCombatState { get; init; }
}

/// <summary>
/// Service pour le système de combat temps réel
/// </summary>
public interface ICombatApiService
{
    Task<CombatStateDto?> StartCombatAsync(StartCombatRequest request);
    Task<CombatStateDto?> GetCombatStateAsync(int combatId);
    Task<CombatStateDto?> AddParticipantAsync(int combatId, CombatParticipantDto participant);
    Task<CombatStateDto?> RollInitiativeAsync(int combatId, int participantId, int initiative);
    Task<CombatStateDto?> StartCombatPhaseAsync(int combatId);
    Task<CombatActionResult?> ExecuteActionAsync(int combatId, CombatActionRequest action);
    Task<CombatStateDto?> AdvanceTurnAsync(int combatId);
    Task<CombatStateDto?> PauseCombatAsync(int combatId, bool isPaused);
    Task<CombatStateDto?> EndCombatAsync(int combatId, string? winner = null);
    Task<List<string>> GetAvailableActionsAsync(int participantId);
}

public class CombatApiService : BaseApiService, ICombatApiService
{
    public CombatApiService(HttpClient httpClient, ILogger<CombatApiService> logger) 
        : base(httpClient, logger)
    {
    }

    /// <summary>
    /// Démarre un nouveau combat
    /// </summary>
    public async Task<CombatStateDto?> StartCombatAsync(StartCombatRequest request)
    {
        try
        {
            _logger.LogInformation("⚔️ Démarrage du combat {CombatName} avec {ParticipantCount} participants", 
                request.Name, request.Participants.Count);

            var combat = await PostAsync<StartCombatRequest, CombatStateDto>("/api/combat/start", request);
            
            if (combat != null)
            {
                _logger.LogInformation("✅ Combat {CombatId} démarré", combat.Id);
                return combat;
            }
            
            _logger.LogWarning("⚠️ Échec du démarrage du combat {CombatName}", request.Name);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors du démarrage du combat {CombatName}", request.Name);
            return null;
        }
    }

    /// <summary>
    /// Récupère l'état actuel d'un combat
    /// </summary>
    public async Task<CombatStateDto?> GetCombatStateAsync(int combatId)
    {
        try
        {
            _logger.LogDebug("📊 Récupération de l'état du combat {CombatId}", combatId);

            var combat = await GetAsync<CombatStateDto>($"/api/combat/{combatId}");
            
            if (combat != null)
            {
                _logger.LogDebug("✅ État du combat {CombatId} récupéré", combatId);
                return combat;
            }
            
            _logger.LogWarning("⚠️ Combat {CombatId} non trouvé", combatId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de la récupération du combat {CombatId}", combatId);
            return null;
        }
    }

    /// <summary>
    /// Ajoute un participant à un combat
    /// </summary>
    public async Task<CombatStateDto?> AddParticipantAsync(int combatId, CombatParticipantDto participant)
    {
        try
        {
            _logger.LogInformation("➕ Ajout du participant {ParticipantName} au combat {CombatId}", 
                participant.Name, combatId);

            var combat = await PostAsync<CombatParticipantDto, CombatStateDto>(
                $"/api/combat/{combatId}/participants", participant);
            
            if (combat != null)
            {
                _logger.LogInformation("✅ Participant {ParticipantName} ajouté au combat {CombatId}", 
                    participant.Name, combatId);
                return combat;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de l'ajout du participant au combat {CombatId}", combatId);
            return null;
        }
    }

    /// <summary>
    /// Lance l'initiative pour un participant
    /// </summary>
    public async Task<CombatStateDto?> RollInitiativeAsync(int combatId, int participantId, int initiative)
    {
        try
        {
            _logger.LogInformation("🎲 Jet d'initiative {Initiative} pour le participant {ParticipantId} dans le combat {CombatId}", 
                initiative, participantId, combatId);

            var request = new { ParticipantId = participantId, InitiativeRoll = initiative };
            var combat = await PostAsync<object, CombatStateDto>($"/api/combat/{combatId}/initiative", request);
            
            if (combat != null)
            {
                _logger.LogInformation("✅ Initiative {Initiative} définie pour le participant {ParticipantId}", 
                    initiative, participantId);
                return combat;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors du jet d'initiative pour le combat {CombatId}", combatId);
            return null;
        }
    }

    /// <summary>
    /// Démarre la phase de combat (après les initiatives)
    /// </summary>
    public async Task<CombatStateDto?> StartCombatPhaseAsync(int combatId)
    {
        try
        {
            _logger.LogInformation("🚀 Démarrage de la phase de combat {CombatId}", combatId);

            var combat = await PostAsync<object, CombatStateDto>($"/api/combat/{combatId}/start-phase", new { });
            
            if (combat != null)
            {
                _logger.LogInformation("✅ Phase de combat {CombatId} démarrée", combatId);
                return combat;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors du démarrage de la phase de combat {CombatId}", combatId);
            return null;
        }
    }

    /// <summary>
    /// Exécute une action de combat
    /// </summary>
    public async Task<CombatActionResult?> ExecuteActionAsync(int combatId, CombatActionRequest action)
    {
        try
        {
            _logger.LogInformation("⚡ Exécution de l'action {ActionType} par l'acteur {ActorId} dans le combat {CombatId}", 
                action.ActionType, action.ActorId, combatId);

            var result = await PostAsync<CombatActionRequest, CombatActionResult>(
                $"/api/combat/{combatId}/actions", action);
            
            if (result != null)
            {
                if (result.Success)
                {
                    _logger.LogInformation("✅ Action {ActionType} réussie: {Description}", 
                        action.ActionType, result.ActionDescription);
                }
                else
                {
                    _logger.LogWarning("❌ Action {ActionType} échouée: {Error}", 
                        action.ActionType, result.ErrorMessage);
                }
                
                return result;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de l'exécution de l'action dans le combat {CombatId}", combatId);
            return null;
        }
    }

    /// <summary>
    /// Passe au tour suivant
    /// </summary>
    public async Task<CombatStateDto?> AdvanceTurnAsync(int combatId)
    {
        try
        {
            _logger.LogInformation("🔄 Avancement au tour suivant pour le combat {CombatId}", combatId);

            var combat = await PostAsync<object, CombatStateDto>($"/api/combat/{combatId}/advance-turn", new { });
            
            if (combat != null)
            {
                _logger.LogInformation("✅ Tour suivant: {CurrentParticipant} (Round {Round})", 
                    combat.CurrentParticipant?.Name, combat.CurrentRound);
                return combat;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de l'avancement du tour pour le combat {CombatId}", combatId);
            return null;
        }
    }

    /// <summary>
    /// Met en pause ou reprend un combat
    /// </summary>
    public async Task<CombatStateDto?> PauseCombatAsync(int combatId, bool isPaused)
    {
        try
        {
            _logger.LogInformation("⏸️ {Action} du combat {CombatId}", isPaused ? "Pause" : "Reprise", combatId);

            var request = new { IsPaused = isPaused };
            var combat = await PostAsync<object, CombatStateDto>($"/api/combat/{combatId}/pause", request);
            
            return combat;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de la pause/reprise du combat {CombatId}", combatId);
            return null;
        }
    }

    /// <summary>
    /// Termine un combat
    /// </summary>
    public async Task<CombatStateDto?> EndCombatAsync(int combatId, string? winner = null)
    {
        try
        {
            _logger.LogInformation("🏁 Fin du combat {CombatId} avec le vainqueur: {Winner}", 
                combatId, winner ?? "Non spécifié");

            var request = new { Winner = winner };
            var combat = await PostAsync<object, CombatStateDto>($"/api/combat/{combatId}/end", request);
            
            if (combat != null)
            {
                _logger.LogInformation("✅ Combat {CombatId} terminé", combatId);
                return combat;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de la fin du combat {CombatId}", combatId);
            return null;
        }
    }

    /// <summary>
    /// Récupère les actions disponibles pour un participant
    /// </summary>
    public async Task<List<string>> GetAvailableActionsAsync(int participantId)
    {
        try
        {
            _logger.LogDebug("📋 Récupération des actions disponibles pour le participant {ParticipantId}", participantId);

            var actions = await GetAsync<List<string>>($"/api/combat/participants/{participantId}/actions");
            
            return actions ?? new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de la récupération des actions pour le participant {ParticipantId}", participantId);
            return new List<string>();
        }
    }
}