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
            [FromBody] UpdateSpellRequest request,
            ICharacterSpellService spellService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                // Créer le bon type de requête pour le service
                var updateRequest = new UpdateCharacterSpellRequest
                {
                    Notes = request.Notes,
                    IsPrepared = request.IsPrepared,
                    SpellSlotLevel = request.SlotLevel,
                    CustomName = request.CustomName
                };

                var updatedSpell = await spellService.UpdateCharacterSpellAsync(characterId, spellId, updateRequest);
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

                return Results.Ok(new { message = request.IsPrepared ? "Sort préparé" : "Sort dépréparé" });
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

                return Results.Ok(new { message = "Niveau de l'emplacement du sort mis à jour" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // ========== ENDPOINTS INVENTAIRE - GROUPE SÉPARÉ ==========
        // Créer un groupe distinct pour éviter les conflits de paramètres
        var characterInventoryGroup = app.MapGroup("/api/characters/{characterId:int}/inventory").RequireAuthorization();

        // GET /api/characters/{characterId}/inventory - Inventaire du personnage
        characterInventoryGroup.MapGet("/", async (
            int characterId,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                var inventory = new
                {
                    characterId,
                    totalWeight = 15.5m,
                    maxWeight = 50.0m,
                    items = new[]
                    {
                        new { id = 1, name = "Épée longue", quantity = 1, equipped = true },
                        new { id = 2, name = "Armure de cuir", quantity = 1, equipped = true },
                        new { id = 3, name = "Potion de soin", quantity = 3, equipped = false }
                    }
                };

                return Results.Ok(inventory);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // POST /api/characters/{characterId}/inventory - Ajouter équipement simplifié
        characterInventoryGroup.MapPost("/", async (
            int characterId,
            [FromBody] SimpleInventoryRequest request,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Implémenter l'ajout à l'inventaire

                var result = new
                {
                    id = Random.Shared.Next(1000, 9999),
                    characterId,
                    equipmentName = request.EquipmentName,
                    quantity = request.Quantity,
                    message = "Équipement ajouté à l'inventaire"
                };

                return Results.Created($"/api/characters/{characterId}/inventory/{result.id}", result);
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

// === MODÈLES DE REQUÊTE LOCAUX ===

public record LearnSpellRequest(string? Notes);
public record PrepareSpellRequest(bool IsPrepared);
public record SetSpellSlotRequest(int? SlotLevel);
public record SimpleInventoryRequest(string EquipmentName, int Quantity);
public record UpdateSpellRequest(string? Notes, bool? IsPrepared, int? SlotLevel, string? CustomName);