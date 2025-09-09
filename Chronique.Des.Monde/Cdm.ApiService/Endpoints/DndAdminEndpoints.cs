using Microsoft.AspNetCore.Mvc;
using Cdm.Business.Dnd.Services;
using System.Security.Claims;

namespace Cdm.ApiService.Endpoints;

/// <summary>
/// Endpoints d'administration pour l'injection de données officielles D&D
/// Accès restreint aux administrateurs uniquement
/// </summary>
public static class DndAdminEndpoints
{
    public static void MapDndAdminEndpoints(this WebApplication app)
    {
        var adminGroup = app.MapGroup("/api/admin/dnd").RequireAuthorization();

        // POST /api/admin/dnd/seed/spells - Injecter les sorts officiels D&D
        adminGroup.MapPost("/seed/spells", async (
            DndOfficialDataSeeder seeder,
            ClaimsPrincipal user) =>
        {
            try
            {
                // TODO: Vérifier que l'utilisateur est administrateur
                var userId = GetUserIdFromClaims(user);
                
                await seeder.SeedOfficialSpellsAsync();
                
                return Results.Ok(new 
                { 
                    message = "Sorts officiels D&D 5e injectés avec succès",
                    seededBy = userId,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // POST /api/admin/dnd/seed/equipment - Injecter les équipements officiels D&D (NOUVEAU)
        adminGroup.MapPost("/seed/equipment", async (
            DndOfficialDataSeeder seeder,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                
                await seeder.SeedOfficialEquipmentAsync();
                
                return Results.Ok(new 
                { 
                    message = "Équipements officiels D&D 5e injectés avec succès",
                    seededBy = userId,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // POST /api/admin/dnd/seed/all - Injecter toutes les données officielles D&D
        adminGroup.MapPost("/seed/all", async (
            DndOfficialDataSeeder seeder,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                
                await seeder.SeedAllOfficialDataAsync();
                
                return Results.Ok(new 
                { 
                    message = "Toutes les données officielles D&D 5e injectées avec succès",
                    seededBy = userId,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/admin/dnd/stats - Statistiques des données officielles
        adminGroup.MapGet("/stats", async (
            DndOfficialDataSeeder seeder,
            ClaimsPrincipal user) =>
        {
            try
            {
                // TODO: Implémenter les statistiques des données
                var stats = new
                {
                    officialSpells = 10, // TODO: Compter réellement
                    officialEquipment = 0, // TODO: Implémenter
                    lastSeeded = DateTime.UtcNow.AddDays(-1), // TODO: Récupérer la vraie date
                    version = "SRD 5.1"
                };
                
                return Results.Ok(stats);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // DELETE /api/admin/dnd/reset - Supprimer toutes les données officielles (DANGER)
        adminGroup.MapDelete("/reset", async (
            DndOfficialDataSeeder seeder,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                
                // TODO: Implémenter la suppression des données officielles
                // ATTENTION: Très dangereux, nécessite des vérifications supplémentaires
                
                return Results.Ok(new 
                { 
                    message = "ATTENTION: Reset non implémenté pour la sécurité",
                    requestedBy = userId,
                    timestamp = DateTime.UtcNow
                });
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