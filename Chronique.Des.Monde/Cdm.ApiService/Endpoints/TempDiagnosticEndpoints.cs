using Cdm.Data.Dnd;
using Cdm.Business.Dnd.Services;
using Microsoft.EntityFrameworkCore;

namespace Cdm.ApiService.Endpoints;

/// <summary>
/// Endpoint temporaire pour diagnostiquer le seeding
/// </summary>
public static class TempDiagnosticEndpoints
{
    public static void MapTempDiagnosticEndpoints(this WebApplication app)
    {
        // Endpoint simple de test
        app.MapGet("/test", () => "API is running!");
        
        // Endpoint pour forcer seeding avec diagnostic étendu
        app.MapPost("/force-seed", async (IServiceProvider serviceProvider, DndDbContext context) =>
        {
            try
            {
                // Vérification avant seeding
                var spellsBefore = await context.SpellsDnd.CountAsync();
                var equipmentBefore = await context.EquipmentDnd.CountAsync();
                
                // Force le seeding
                await serviceProvider.SeedDndOfficialDataAsync();
                
                // Vérification après seeding
                var spellsAfter = await context.SpellsDnd.CountAsync();
                var equipmentAfter = await context.EquipmentDnd.CountAsync();
                
                return Results.Ok(new
                {
                    Message = "Force seeding completed",
                    Before = new { Spells = spellsBefore, Equipment = equipmentBefore },
                    After = new { Spells = spellsAfter, Equipment = equipmentAfter },
                    Changes = new { 
                        SpellsAdded = spellsAfter - spellsBefore, 
                        EquipmentAdded = equipmentAfter - equipmentBefore 
                    }
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        });
    }
}