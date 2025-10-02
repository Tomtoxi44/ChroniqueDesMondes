using Cdm.Data.Dnd;
using Cdm.Business.Dnd.Services;
using Microsoft.EntityFrameworkCore;

namespace Cdm.ApiService.Endpoints;

public static class DataSeedingTestEndpoints
{
    public static void MapDataSeedingTestEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/test/seeding")
            .WithTags("Data Seeding Test")
            .WithOpenApi();

        // Test endpoint pour vérifier si les données D&D sont seedées
        group.MapGet("/status", async (DndDbContext context) =>
        {
            var spellCount = await context.SpellsDnd.CountAsync();
            var equipmentCount = await context.EquipmentDnd.CountAsync();
            
            return Results.Ok(new
            {
                SpellsCount = spellCount,
                EquipmentCount = equipmentCount,
                HasData = spellCount > 0 || equipmentCount > 0,
                Message = spellCount > 0 || equipmentCount > 0 
                    ? "✅ Data seeding appears to be working"
                    : "❌ No D&D data found - seeding may not be running"
            });
        });

        // Endpoint pour forcer le seeding manuellement
        group.MapPost("/force", async (IServiceProvider serviceProvider) =>
        {
            try
            {
                await serviceProvider.SeedDndOfficialDataAsync();
                return Results.Ok(new { Message = "✅ Manual seeding completed successfully" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { Message = $"❌ Seeding failed: {ex.Message}" });
            }
        });
    }
}