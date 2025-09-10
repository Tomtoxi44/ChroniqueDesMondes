using Cdm.Business.Common.Interfaces.Combat;
using Cdm.Business.Common.Models.Combat;
using Microsoft.AspNetCore.Mvc;

namespace Cdm.ApiService.Endpoints;

/// <summary>
/// Endpoints API pour le système de combat temps réel
/// Gère le démarrage, la progression et les actions de combat
/// </summary>
public static class CombatEndpoints
{
    public static void MapCombatEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/combat")
            .WithTags("Combat")
            .WithOpenApi();

        // Démarrage et gestion des combats
        group.MapPost("/start", StartCombat)
            .WithName("StartCombat")
            .WithSummary("Démarre un nouveau combat")
            .Produces<CombatStateDto>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapGet("/{combatId:int}", GetCombatState)
            .WithName("GetCombatState")
            .WithSummary("Obtient l'état actuel d'un combat")
            .Produces<CombatStateDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        group.MapPost("/{combatId:int}/participants", AddParticipant)
            .WithName("AddParticipantToCombat")
            .WithSummary("Ajoute un participant à un combat en cours")
            .Produces<CombatStateDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        // Gestion de l'initiative
        group.MapPost("/{combatId:int}/initiative", RollInitiative)
            .WithName("RollInitiative")
            .WithSummary("Lance l'initiative pour un participant")
            .Produces<CombatStateDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("/{combatId:int}/start-phase", StartCombatPhase)
            .WithName("StartCombatPhase")
            .WithSummary("Démarre la phase de combat après les initiatives")
            .Produces<CombatStateDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        // Actions de combat
        group.MapPost("/{combatId:int}/actions", ExecuteAction)
            .WithName("ExecuteCombatAction")
            .WithSummary("Exécute une action de combat")
            .Produces<CombatActionResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        // Gestion des tours
        group.MapPost("/{combatId:int}/advance-turn", AdvanceTurn)
            .WithName("AdvanceTurn")
            .WithSummary("Passe au tour suivant")
            .Produces<CombatStateDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        // Contrôle du combat
        group.MapPost("/{combatId:int}/pause", PauseCombat)
            .WithName("PauseCombat")
            .WithSummary("Met en pause ou reprend un combat")
            .Produces<CombatStateDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("/{combatId:int}/end", EndCombat)
            .WithName("EndCombat")
            .WithSummary("Termine un combat")
            .Produces<CombatStateDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        // Effets de statut
        group.MapPost("/participants/{participantId:int}/status-effects", ApplyStatusEffect)
            .WithName("ApplyStatusEffect")
            .WithSummary("Applique un effet de statut à un participant")
            .Produces<StatusEffectResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapDelete("/status-effects/{effectId:int}", RemoveStatusEffect)
            .WithName("RemoveStatusEffect")
            .WithSummary("Supprime un effet de statut")
            .Produces<bool>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        // Utilitaires
        group.MapGet("/participants/{participantId:int}/actions", GetAvailableActions)
            .WithName("GetAvailableActions")
            .WithSummary("Obtient les actions disponibles pour un participant")
            .Produces<List<string>>(StatusCodes.Status200OK);

        group.MapGet("/{combatId:int}/can-act/{participantId:int}", CanParticipantAct)
            .WithName("CanParticipantAct")
            .WithSummary("Vérifie si un participant peut agir")
            .Produces<object>(StatusCodes.Status200OK);
    }

    /// <summary>
    /// Démarre un nouveau combat
    /// </summary>
    private static async Task<IResult> StartCombat(
        [FromBody] StartCombatCommand command,
        ICombatEngine combatEngine,
        ILogger<ICombatEngine> logger)
    {
        try
        {
            logger.LogInformation("🎯 Début de combat: {Name} avec {Count} participants", 
                command.Name, command.Participants.Count);

            var result = await combatEngine.StartCombatAsync(command);
            
            logger.LogInformation("⚔️ Combat {CombatId} créé avec succès", result.Id);
            
            return Results.Created($"/api/combat/{result.Id}", result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors du démarrage du combat {Name}", command.Name);
            return Results.Problem(
                title: "Erreur de démarrage du combat",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Obtient l'état actuel d'un combat
    /// </summary>
    private static async Task<IResult> GetCombatState(
        int combatId,
        ICombatEngine combatEngine,
        ILogger<ICombatEngine> logger)
    {
        try
        {
            var result = await combatEngine.GetCombatStateAsync(combatId);
            
            if (result == null)
            {
                logger.LogWarning("Combat {CombatId} non trouvé", combatId);
                return Results.NotFound($"Combat {combatId} non trouvé");
            }

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la récupération du combat {CombatId}", combatId);
            return Results.Problem(
                title: "Erreur de récupération du combat",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Ajoute un participant à un combat en cours
    /// </summary>
    private static async Task<IResult> AddParticipant(
        int combatId,
        [FromBody] CombatParticipantDto participant,
        ICombatEngine combatEngine,
        ILogger<ICombatEngine> logger)
    {
        try
        {
            logger.LogInformation("🆕 Ajout participant au combat {CombatId}: {ParticipantType}", 
                combatId, participant.ParticipantType);

            var result = await combatEngine.AddParticipantAsync(combatId, participant);
            
            logger.LogInformation("✅ Participant ajouté au combat {CombatId}", combatId);
            
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de l'ajout du participant au combat {CombatId}", combatId);
            return Results.Problem(
                title: "Erreur d'ajout de participant",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Lance l'initiative pour un participant
    /// </summary>
    private static async Task<IResult> RollInitiative(
        int combatId,
        [FromBody] RollInitiativeRequest request,
        ICombatEngine combatEngine,
        ILogger<ICombatEngine> logger)
    {
        try
        {
            logger.LogInformation("🎲 Jet d'initiative {Initiative} pour participant {ParticipantId} dans combat {CombatId}", 
                request.InitiativeRoll, request.ParticipantId, combatId);

            var result = await combatEngine.RollInitiativeAsync(combatId, request.ParticipantId, request.InitiativeRoll);
            
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors du jet d'initiative pour combat {CombatId}", combatId);
            return Results.Problem(
                title: "Erreur de jet d'initiative",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Démarre la phase de combat après les initiatives
    /// </summary>
    private static async Task<IResult> StartCombatPhase(
        int combatId,
        ICombatEngine combatEngine,
        ILogger<ICombatEngine> logger)
    {
        try
        {
            logger.LogInformation("⚔️ Démarrage phase de combat {CombatId}", combatId);

            var result = await combatEngine.StartCombatPhaseAsync(combatId);
            
            logger.LogInformation("🚀 Combat {CombatId} démarré ! Tour de: {CurrentParticipant}", 
                combatId, result.CurrentParticipant?.Name);
            
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors du démarrage de la phase de combat {CombatId}", combatId);
            return Results.Problem(
                title: "Erreur de démarrage de phase",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Exécute une action de combat
    /// </summary>
    private static async Task<IResult> ExecuteAction(
        int combatId,
        [FromBody] ExecuteCombatActionCommand command,
        ICombatEngine combatEngine,
        ILogger<ICombatEngine> logger)
    {
        try
        {
            // S'assurer que le combatId correspond
            command.CombatId = combatId;

            logger.LogInformation("⚡ Exécution action {ActionType} par acteur {ActorId} dans combat {CombatId}", 
                command.ActionType, command.ActorId, combatId);

            var result = await combatEngine.ExecuteActionAsync(command);
            
            if (result.Success)
            {
                logger.LogInformation("✅ Action {ActionType} réussie: {Description}", 
                    command.ActionType, result.ActionDescription);
            }
            else
            {
                logger.LogWarning("❌ Action {ActionType} échouée: {Error}", 
                    command.ActionType, result.ErrorMessage);
            }
            
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de l'exécution d'action dans combat {CombatId}", combatId);
            return Results.Problem(
                title: "Erreur d'exécution d'action",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Passe au tour suivant
    /// </summary>
    private static async Task<IResult> AdvanceTurn(
        int combatId,
        ICombatEngine combatEngine,
        ILogger<ICombatEngine> logger)
    {
        try
        {
            logger.LogInformation("🔄 Avancement au tour suivant dans combat {CombatId}", combatId);

            var result = await combatEngine.AdvanceToNextTurnAsync(combatId);
            
            logger.LogInformation("➡️ Nouveau tour: {CurrentParticipant} (Round {Round})", 
                result.CurrentParticipant?.Name, result.CurrentRound);
            
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de l'avancement du tour dans combat {CombatId}", combatId);
            return Results.Problem(
                title: "Erreur d'avancement de tour",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Met en pause ou reprend un combat
    /// </summary>
    private static async Task<IResult> PauseCombat(
        int combatId,
        [FromBody] PauseCombatRequest request,
        ICombatEngine combatEngine,
        ILogger<ICombatEngine> logger)
    {
        try
        {
            logger.LogInformation("⏸️ {Action} combat {CombatId}", 
                request.IsPaused ? "Pause" : "Reprise", combatId);

            var result = await combatEngine.PauseCombatAsync(combatId, request.IsPaused);
            
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la pause/reprise du combat {CombatId}", combatId);
            return Results.Problem(
                title: "Erreur de contrôle du combat",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Termine un combat
    /// </summary>
    private static async Task<IResult> EndCombat(
        int combatId,
        [FromBody] EndCombatRequest? request,
        ICombatEngine combatEngine,
        ILogger<ICombatEngine> logger)
    {
        try
        {
            logger.LogInformation("🏁 Fin du combat {CombatId}", combatId);

            var result = await combatEngine.EndCombatAsync(combatId, request?.Winner);
            
            logger.LogInformation("✅ Combat {CombatId} terminé. Vainqueur: {Winner}", 
                combatId, request?.Winner ?? "Non spécifié");
            
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la fin du combat {CombatId}", combatId);
            return Results.Problem(
                title: "Erreur de fin de combat",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Applique un effet de statut à un participant
    /// </summary>
    private static async Task<IResult> ApplyStatusEffect(
        int participantId,
        [FromBody] StatusEffectDto effect,
        ICombatEngine combatEngine,
        ILogger<ICombatEngine> logger)
    {
        try
        {
            logger.LogInformation("✨ Application effet {EffectName} à participant {ParticipantId}", 
                effect.Name, participantId);

            var result = await combatEngine.ApplyStatusEffectAsync(participantId, effect);
            
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de l'application d'effet à participant {ParticipantId}", participantId);
            return Results.Problem(
                title: "Erreur d'application d'effet",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Supprime un effet de statut
    /// </summary>
    private static async Task<IResult> RemoveStatusEffect(
        int effectId,
        ICombatEngine combatEngine,
        ILogger<ICombatEngine> logger)
    {
        try
        {
            logger.LogInformation("🧹 Suppression effet {EffectId}", effectId);

            var result = await combatEngine.RemoveStatusEffectAsync(effectId);
            
            if (!result)
            {
                return Results.NotFound($"Effet {effectId} non trouvé");
            }
            
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la suppression d'effet {EffectId}", effectId);
            return Results.Problem(
                title: "Erreur de suppression d'effet",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Obtient les actions disponibles pour un participant
    /// </summary>
    private static async Task<IResult> GetAvailableActions(
        int participantId,
        ICombatEngine combatEngine)
    {
        var actions = await combatEngine.GetAvailableActionsAsync(participantId);
        return Results.Ok(actions);
    }

    /// <summary>
    /// Vérifie si un participant peut agir
    /// </summary>
    private static async Task<IResult> CanParticipantAct(
        int combatId,
        int participantId,
        ICombatEngine combatEngine)
    {
        var (canAct, reason) = await combatEngine.CanParticipantActAsync(combatId, participantId);
        
        return Results.Ok(new { CanAct = canAct, Reason = reason });
    }
}

#region DTOs pour les requêtes

/// <summary>
/// Requête pour lancer l'initiative
/// </summary>
public record RollInitiativeRequest(int ParticipantId, int InitiativeRoll);

/// <summary>
/// Requête pour mettre en pause un combat
/// </summary>
public record PauseCombatRequest(bool IsPaused);

/// <summary>
/// Requête pour terminer un combat
/// </summary>
public record EndCombatRequest(string? Winner);

#endregion