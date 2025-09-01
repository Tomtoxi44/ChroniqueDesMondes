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
/// Suit exactement le pattern des équipements avec injection par clé
/// </summary>
public static class SpellEndpoint
{
    public static void MapSpellEndpoints(this WebApplication app)
    {
        var spellGroup = app.MapGroup("/api/spells").RequireAuthorization();

        // GET /api/spells?userId={id}&gameType={type} - Sorts officiels + privés utilisateur (EXISTANT)
        spellGroup.MapGet("/", async (
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

                var spells = await spellBusiness.GetAllSpellsByUserId(userId, gameType);
                return Results.Ok(spells);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/spells/official?gameType={type} - Sorts officiels uniquement (NOUVEAU)
        spellGroup.MapGet("/official", async (
            GameType gameType,
            [FromKeyedServices(DndBusinessExtensions.DndKey)] ISpellBusiness spellBusiness,
            ClaimsPrincipal user) =>
        {
            try
            {
                var spells = await spellBusiness.GetOfficialSpellsAsync(gameType);
                return Results.Ok(spells);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/spells/user?gameType={type}&userId={id} - Sorts privés utilisateur uniquement (NOUVEAU)
        spellGroup.MapGet("/user", async (
            GameType gameType,
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

                var spells = await spellBusiness.GetUserPrivateSpellsAsync(userId, gameType);
                return Results.Ok(spells);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/spells/{id}?userId={userId} - Détails d'un sort (EXISTANT)
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

        // POST /api/spells?userId={id} - Création d'un sort privé utilisateur (EXISTANT mais amélioré)
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

        // POST /api/spells/dnd?userId={id} - Création D&D avec propriétés spécialisées (EXISTANT)
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
                        { "Level", request.Level },
                        { "School", request.School },
                        { "CastingTime", request.CastingTime },
                        { "Range", request.Range },
                        { "Components", request.Components },
                        { "Duration", request.Duration },
                        { "IsRitual", request.IsRitual },
                        { "RequiresConcentration", request.RequiresConcentration },
                        { "Damage", request.Damage ?? "" },
                        { "DamageType", request.DamageType ?? "" },
                        { "SaveType", request.SaveType ?? "" },
                        { "AttackType", request.AttackType ?? "" }
                    }
                };

                var spell = await spellBusiness.CreateSpell(spellRequest, userId);
                return Results.Created($"/api/spells/{spell.Id}", spell);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT /api/spells/{id} - Modification d'un sort (EXISTANT)
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

        // DELETE /api/spells/{id} - Suppression d'un sort (EXISTANT)
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

        // GET /api/spells/search?q={text}&userId={id}&gameType={type} - Recherche de sorts (EXISTANT)
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