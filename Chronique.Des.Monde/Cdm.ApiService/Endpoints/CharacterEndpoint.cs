namespace Chronique.Des.Mondes.ApiService.Endpoints;

using Cdm.Common;
using Cmd.Abstraction;
using Cmd.Abstraction.ModelsRequest;
using Cmd.Business.Character.ModelsRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

public static class CharacterEndpoint
{
    public static void MapPlayerCharacterEndpoint(this WebApplication app)
    {
        var characterGroup = app.MapGroup("/character").RequireAuthorization();

        // GET /character - Récupérer tous les personnages d'un utilisateur
        characterGroup.MapGet(string.Empty, async (int userId, [FromHeader(Name = "X-GameType")] string gameType, IServiceProvider serviceProvider) =>
        {
            try
            {
                var serviceCharacter = serviceProvider.GetRequiredKeyedService<ICharacterBusiness>(gameType);
                var result = await serviceCharacter.GetAllCharactersByUserId(userId);
                return Results.Ok(result);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // GET /character/{id} - Récupérer un personnage par ID
        characterGroup.MapGet("/{id:int}", async (int id, [FromHeader(Name = "X-GameType")] string gameType, IServiceProvider serviceProvider) =>
        {
            try
            {
                var serviceCharacter = serviceProvider.GetRequiredKeyedService<ICharacterBusiness>(gameType);
                var result = await serviceCharacter.GetCharacterById(id);
                return Results.Ok(result);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // POST /character - Créer un nouveau personnage (générique)
        characterGroup.MapPost(string.Empty, async (int userId, [FromHeader(Name = "X-GameType")] string gameType, [FromBody] CharacterRequest characterRequest, IServiceProvider serviceProvider) =>
        {
            try
            {
                var serviceCharacter = serviceProvider.GetRequiredKeyedService<ICharacterBusiness>(gameType);
                var result = await serviceCharacter.CreateCharacter(characterRequest, userId);
                return Results.Created($"/character/{result.Id}", result);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // POST /character/dnd - Créer un nouveau personnage D&D (option spécifique pour compatibilité)
        characterGroup.MapPost("/dnd", async (int userId, [FromHeader(Name = "X-GameType")] string gameType, [FromBody] CharacterDndRequest dndRequest, IServiceProvider serviceProvider) =>
        {
            try
            {
                var serviceCharacter = serviceProvider.GetRequiredKeyedService<ICharacterBusiness>(gameType);
                
                // Conversion du CharacterDndRequest vers le format générique
                var genericRequest = CharacterRequestFactory.CreateDndCharacterRequest(
                    dndRequest.Name, dndRequest.Leveling, dndRequest.Life,
                    dndRequest.Picture, dndRequest.Background, dndRequest.Class,
                    dndRequest.ClassArmor, dndRequest.Strong, dndRequest.Dexterity,
                    dndRequest.Constitution, dndRequest.Intelligence, dndRequest.Wisdoms, dndRequest.Charism);
                
                var result = await serviceCharacter.CreateCharacter(genericRequest, userId);
                return Results.Created($"/character/{result.Id}", result);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // PUT /character/{id} - Mettre à jour un personnage
        characterGroup.MapPut("/{id:int}", async (int id, [FromHeader(Name = "X-GameType")] string gameType, [FromBody] CharacterRequest characterRequest, IServiceProvider serviceProvider) =>
        {
            try
            {
                var serviceCharacter = serviceProvider.GetRequiredKeyedService<ICharacterBusiness>(gameType);
                var result = await serviceCharacter.UpdateCharacter(characterRequest, id);
                return Results.Ok(result);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // PUT /character/dnd/{id} - Mettre à jour un personnage D&D (option spécifique pour compatibilité)
        characterGroup.MapPut("/dnd/{id:int}", async (int id, [FromHeader(Name = "X-GameType")] string gameType, [FromBody] CharacterDndRequest dndRequest, IServiceProvider serviceProvider) =>
        {
            try
            {
                var serviceCharacter = serviceProvider.GetRequiredKeyedService<ICharacterBusiness>(gameType);
                
                // Conversion du CharacterDndRequest vers le format générique
                var genericRequest = CharacterRequestFactory.CreateDndCharacterRequest(
                    dndRequest.Name, dndRequest.Leveling, dndRequest.Life,
                    dndRequest.Picture, dndRequest.Background, dndRequest.Class,
                    dndRequest.ClassArmor, dndRequest.Strong, dndRequest.Dexterity,
                    dndRequest.Constitution, dndRequest.Intelligence, dndRequest.Wisdoms, dndRequest.Charism);
                
                var result = await serviceCharacter.UpdateCharacter(genericRequest, id);
                return Results.Ok(result);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // DELETE /character/{id} - Supprimer un personnage
        characterGroup.MapDelete("/{id:int}", async (int id, [FromHeader(Name = "X-GameType")] string gameType, IServiceProvider serviceProvider) =>
        {
            try
            {
                var serviceCharacter = serviceProvider.GetRequiredKeyedService<ICharacterBusiness>(gameType);
                await serviceCharacter.DeleteCharacter(id);
                return Results.NoContent();
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });
    }
}

