using Microsoft.AspNetCore.Mvc;
using Cmd.Abstraction.Characters;
using System.Security.Claims;

namespace Cdm.ApiService.Endpoints;

/// <summary>
/// Endpoints pour l'attribution et la gestion des sorts d'un personnage
/// </summary>
public static class CharacterSpellEndpoints
{
    public static void MapCharacterSpellEndpoints(this WebApplication app)
    {
        var characterSpellGroup = app.MapGroup("/api/characters/{characterId:int}/spells").RequireAuthorization();

        // GET /api/characters/{characterId}/spells - Liste des sorts du personnage
        characterSpellGroup.MapGet("/", async (
            int characterId,
            ICharacterSpellService spellService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                var spells = await spellService.GetCharacterSpellsAsync(characterId);
                return Results.Ok(spells);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/characters/{characterId}/spells/prepared - Sorts préparés du personnage
        characterSpellGroup.MapGet("/prepared", async (
            int characterId,
            ICharacterSpellService spellService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                var preparedSpells = await spellService.GetPreparedSpellsAsync(characterId);
                return Results.Ok(preparedSpells);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/characters/{characterId}/spells/available - Sorts disponibles pour le personnage
        characterSpellGroup.MapGet("/available", async (
            int characterId,
            ICharacterSpellService spellService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                var availableSpells = await spellService.GetAvailableSpellsForCharacterAsync(characterId, userId);
                return Results.Ok(availableSpells);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/characters/{characterId}/spells/search?q={searchText} - Recherche de sorts compatibles
        characterSpellGroup.MapGet("/search", async (
            int characterId,
            string q,
            ICharacterSpellService spellService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                var compatibleSpells = await spellService.SearchCompatibleSpellsAsync(characterId, q, userId);
                return Results.Ok(compatibleSpells);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // POST /api/characters/{characterId}/spells/{spellId} - Apprendre un sort
        characterSpellGroup.MapPost("/{spellId:int}", async (
            int characterId,
            int spellId,
            [FromBody] LearnSpellRequest? request,
            ICharacterSpellService spellService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                var characterSpell = await spellService.AddSpellToCharacterAsync(characterId, spellId, request?.Notes);
                return Results.Created($"/api/characters/{characterId}/spells/{spellId}", characterSpell);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT /api/characters/{characterId}/spells/{spellId} - Mettre à jour un sort du personnage
        characterSpellGroup.MapPut("/{spellId:int}", async (
            int characterId,
            int spellId,
            [FromBody] UpdateCharacterSpellRequest request,
            ICharacterSpellService spellService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                var updatedSpell = await spellService.UpdateCharacterSpellAsync(characterId, spellId, request);
                return Results.Ok(updatedSpell);
            }
            catch (InvalidOperationException)
            {
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // DELETE /api/characters/{characterId}/spells/{spellId} - Oublier un sort
        characterSpellGroup.MapDelete("/{spellId:int}", async (
            int characterId,
            int spellId,
            ICharacterSpellService spellService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                await spellService.RemoveSpellFromCharacterAsync(characterId, spellId);
                return Results.NoContent();
            }
            catch (InvalidOperationException)
            {
                return Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT /api/characters/{characterId}/spells/{spellId}/prepare - Préparer/dépréparer un sort
        characterSpellGroup.MapPut("/{spellId:int}/prepare", async (
            int characterId,
            int spellId,
            [FromBody] PrepareSpellRequest request,
            ICharacterSpellService spellService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                var success = await spellService.PrepareSpellAsync(characterId, spellId, request.IsPrepared);
                if (!success)
                {
                    return Results.NotFound();
                }

                return Results.Ok(new { message = request.IsPrepared ? "Spell prepared" : "Spell unprepared" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT /api/characters/{characterId}/spells/{spellId}/slot-level - Définir le niveau d'emplacement
        characterSpellGroup.MapPut("/{spellId:int}/slot-level", async (
            int characterId,
            int spellId,
            [FromBody] SetSpellSlotRequest request,
            ICharacterSpellService spellService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                var success = await spellService.SetSpellSlotLevelAsync(characterId, spellId, request.SlotLevel);
                if (!success)
                {
                    return Results.NotFound();
                }

                return Results.Ok(new { message = "Spell slot level updated" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });
    }

    private static int GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("Invalid user token");
    }
}

// === MODÈLES DE REQUÊTE ===

public record LearnSpellRequest(string? Notes);
public record PrepareSpellRequest(bool IsPrepared);
public record SetSpellSlotRequest(int? SlotLevel);