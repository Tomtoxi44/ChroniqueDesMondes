using Microsoft.AspNetCore.Mvc;
using Cdm.Data.Dnd.Models;
using Cdm.Data.Dnd;
using Microsoft.EntityFrameworkCore;

namespace Cdm.ApiService.Endpoints;

public static class CharacterEndpoint
{
    public static void MapCharacterEndpoints(this WebApplication app)
    {
        var characterGroup = app.MapGroup("/api/characters").RequireAuthorization();

        characterGroup.MapGet("/", async (DndDbContext context) =>
        {
            var characters = await context.CharactersDnd
                .Where(c => !c.IsNpc)
                .ToListAsync();
            return Results.Ok(characters);
        });

        characterGroup.MapGet("/{id:int}", async (int id, DndDbContext context) =>
        {
            var character = await context.CharactersDnd
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsNpc);
            
            return character != null ? Results.Ok(character) : Results.NotFound();
        });

        characterGroup.MapPost("/", async ([FromBody] CharacterDnd character, DndDbContext context) =>
        {
            character.IsNpc = false; // Ensure it's not an NPC
            context.CharactersDnd.Add(character);
            await context.SaveChangesAsync();
            return Results.Created($"/api/characters/{character.Id}", character);
        });

        characterGroup.MapPut("/{id:int}", async (int id, [FromBody] CharacterDnd updatedCharacter, DndDbContext context) =>
        {
            var character = await context.CharactersDnd
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsNpc);
            
            if (character == null)
                return Results.NotFound();

            // Update properties
            character.Name = updatedCharacter.Name;
            character.Class = updatedCharacter.Class;
            character.Life = updatedCharacter.Life;
            character.Leveling = updatedCharacter.Leveling;
            character.Strong = updatedCharacter.Strong;
            character.Dexterity = updatedCharacter.Dexterity;
            character.Constitution = updatedCharacter.Constitution;
            character.Intelligence = updatedCharacter.Intelligence;
            character.Wisdoms = updatedCharacter.Wisdoms;
            character.Charism = updatedCharacter.Charism;
            character.ClassArmor = updatedCharacter.ClassArmor;
            character.Background = updatedCharacter.Background;
            character.Picture = updatedCharacter.Picture;

            await context.SaveChangesAsync();
            return Results.Ok(character);
        });

        characterGroup.MapDelete("/{id:int}", async (int id, DndDbContext context) =>
        {
            var character = await context.CharactersDnd
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsNpc);
            
            if (character == null)
                return Results.NotFound();

            context.CharactersDnd.Remove(character);
            await context.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}

