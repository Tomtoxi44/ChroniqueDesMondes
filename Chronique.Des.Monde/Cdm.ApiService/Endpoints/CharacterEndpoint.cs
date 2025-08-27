using Microsoft.AspNetCore.Mvc;
using Cdm.Data.Dnd.Models;
using Cdm.Data.Dnd;
using Microsoft.EntityFrameworkCore;
using Cmd.Abstraction;
using Cmd.Abstraction.ModelsRequest;
using Cdm.Business.Dnd.ModelsRequest;
using Cdm.Business.Dnd.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Cdm.ApiService.Endpoints;

public static class CharacterEndpoint
{
    public static void MapCharacterEndpoints(this WebApplication app)
    {
        var characterGroup = app.MapGroup("/api/characters").RequireAuthorization();

        // GET /api/characters?userId={id} - Liste des personnages d'un utilisateur
        characterGroup.MapGet("/", async (int userId, [FromKeyedServices(DndBusinessExtensions.DndKey)] ICharacterBusiness characterBusiness) =>
        {
            try
            {
                var characters = await characterBusiness.GetAllCharactersByUserId(userId);
                return Results.Ok(characters);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/characters/{id} - Détails d'un personnage
        characterGroup.MapGet("/{id:int}", async (int id, [FromKeyedServices(DndBusinessExtensions.DndKey)] ICharacterBusiness characterBusiness) =>
        {
            try
            {
                var character = await characterBusiness.GetCharacterById(id);
                return Results.Ok(character);
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

        // POST /api/characters?userId={id} - Création d'un personnage D&D
        characterGroup.MapPost("/", async (int userId, [FromBody] CharacterRequest request, [FromKeyedServices(DndBusinessExtensions.DndKey)] ICharacterBusiness characterBusiness) =>
        {
            try
            {
                var character = await characterBusiness.CreateCharacter(request, userId);
                return Results.Created($"/api/characters/{character.Id}", character);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // POST /api/characters/dnd?userId={id} - Création D&D avec factory
        characterGroup.MapPost("/dnd", async (int userId, [FromBody] CharacterDndRequest request, [FromKeyedServices(DndBusinessExtensions.DndKey)] ICharacterBusiness characterBusiness) =>
        {
            try
            {
                var characterRequest = CharacterRequestFactory.CreateDndCharacterRequest(
                    name: request.Name,
                    leveling: request.Leveling,
                    life: request.Life,
                    picture: request.Picture,
                    background: request.Background,
                    characterClass: request.Class,
                    classArmor: request.ClassArmor,
                    strong: request.Strong,
                    dexterity: request.Dexterity,
                    constitution: request.Constitution,
                    intelligence: request.Intelligence,
                    wisdoms: request.Wisdoms,
                    charism: request.Charism
                );

                var character = await characterBusiness.CreateCharacter(characterRequest, userId);
                return Results.Created($"/api/characters/{character.Id}", character);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT /api/characters/{id} - Modification d'un personnage
        characterGroup.MapPut("/{id:int}", async (int id, [FromBody] CharacterRequest request, [FromKeyedServices(DndBusinessExtensions.DndKey)] ICharacterBusiness characterBusiness) =>
        {
            try
            {
                var character = await characterBusiness.UpdateCharacter(request, id);
                return Results.Ok(character);
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

        // DELETE /api/characters/{id} - Suppression d'un personnage
        characterGroup.MapDelete("/{id:int}", async (int id, [FromKeyedServices(DndBusinessExtensions.DndKey)] ICharacterBusiness characterBusiness) =>
        {
            try
            {
                await characterBusiness.DeleteCharacter(id);
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
    }
}

