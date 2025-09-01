using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Cmd.Abstraction.Spells;
using Cdm.Business.Dnd.Extensions;
using Cdm.Business.Dnd.Models.Spells;
using Cdm.Common.Enums;
using System.Security.Claims;

namespace Cdm.ApiService.Endpoints;

/// <summary>
/// Endpoints pour la gestion des sorts - API générique qui redispatch selon le GameType
/// Suit exactement le pattern CharacterEndpoint avec injection par clé
/// </summary>
public static class SpellEndpoint
{
    public static void MapSpellEndpoints(this WebApplication app)
    {
        var spellGroup = app.MapGroup("/api/spells").RequireAuthorization();

        // GET /api/spells?userId={id}&gameType={type} - Liste des sorts d'un utilisateur
        spellGroup.MapGet("/", async (
            int userId, 
            GameType gameType,
            [FromKeyedServices(DndBusinessExtensions.DndKey)] ISpellBusiness spellBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var requestingUserId = GetUserIdFromClaims(user);
                
                // Validation : un utilisateur ne peut voir que ses propres sorts
                if (requestingUserId != userId)
                {
                    return Results.Forbid();
                }

                var spells = await spellBusiness.GetAllSpellsByUserId(userId, gameType);
                return Results.Ok(spells);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/spells/{id}?userId={userId} - Détails d'un sort
        spellGroup.MapGet("/{id:int}", async (
            int id, 
            int userId,
            [FromKeyedServices(DndBusinessExtensions.DndKey)] ISpellBusiness spellBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var requestingUserId = GetUserIdFromClaims(user);
                
                if (requestingUserId != userId)
                {
                    return Results.Forbid();
                }

                var spell = await spellBusiness.GetSpellById(id, userId);
                return Results.Ok(spell);
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

        // POST /api/spells?userId={id} - Création d'un sort générique
        spellGroup.MapPost("/", async (
            int userId, 
            [FromBody] SpellRequest request, 
            [FromKeyedServices(DndBusinessExtensions.DndKey)] ISpellBusiness spellBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var requestingUserId = GetUserIdFromClaims(user);
                
                if (requestingUserId != userId)
                {
                    return Results.Forbid();
                }

                var spell = await spellBusiness.CreateSpell(request, userId);
                return Results.Created($"/api/spells/{spell.Id}", spell);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // POST /api/spells/dnd?userId={id} - Création D&D avec propriétés spécialisées
        spellGroup.MapPost("/dnd", async (
            int userId, 
            [FromBody] SpellDndRequest request, 
            [FromKeyedServices(DndBusinessExtensions.DndKey)] ISpellBusiness spellBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var requestingUserId = GetUserIdFromClaims(user);
                
                if (requestingUserId != userId)
                {
                    return Results.Forbid();
                }

                // Convertir SpellDndRequest en SpellRequest générique avec propriétés spécialisées
                var spellRequest = new SpellRequest
                {
                    Name = request.Name,
                    Description = request.Description,
                    ImageUrl = request.ImageUrl,
                    GameType = GameType.DnD,
                    Tags = request.Tags,
                    SpecializedProperties = new Dictionary<string, object>
                    {
                        { "School", request.School },
                        { "Level", request.Level },
                        { "CastingTime", request.CastingTime },
                        { "Range", request.Range },
                        { "Duration", request.Duration },
                        { "Components", request.Components },
                        { "AttackRoll", request.AttackRoll ?? "" },
                        { "Damage", request.Damage ?? "" },
                        { "SavingThrow", request.SavingThrow ?? "" },
                        { "IsRitual", request.IsRitual },
                        { "RequiresConcentration", request.RequiresConcentration },
                        { "MaterialComponent", request.MaterialComponent ?? "" }
                    }
                };

                if (request.HigherLevelDamage.HasValue)
                {
                    spellRequest.SpecializedProperties["HigherLevelDamage"] = request.HigherLevelDamage.Value;
                }

                var spell = await spellBusiness.CreateSpell(spellRequest, userId);
                return Results.Created($"/api/spells/{spell.Id}", spell);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT /api/spells/{id} - Modification d'un sort
        spellGroup.MapPut("/{id:int}", async (
            int id, 
            [FromBody] SpellRequest request, 
            [FromKeyedServices(DndBusinessExtensions.DndKey)] ISpellBusiness spellBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var spell = await spellBusiness.UpdateSpell(request, id, userId);
                return Results.Ok(spell);
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

        // DELETE /api/spells/{id} - Suppression d'un sort
        spellGroup.MapDelete("/{id:int}", async (
            int id, 
            [FromKeyedServices(DndBusinessExtensions.DndKey)] ISpellBusiness spellBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                await spellBusiness.DeleteSpell(id, userId);
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

        // GET /api/spells/search?q={text}&userId={id}&gameType={type} - Recherche de sorts
        spellGroup.MapGet("/search", async (
            string q,
            int userId,
            GameType gameType,
            [FromKeyedServices(DndBusinessExtensions.DndKey)] ISpellBusiness spellBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var requestingUserId = GetUserIdFromClaims(user);
                
                if (requestingUserId != userId)
                {
                    return Results.Forbid();
                }

                var spells = await spellBusiness.SearchSpells(q, userId, gameType);
                return Results.Ok(spells);
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