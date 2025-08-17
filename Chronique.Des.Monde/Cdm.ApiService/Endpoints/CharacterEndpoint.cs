namespace Chronique.Des.Mondes.ApiService.Endpoints;

using Cmd.Abstraction;
using Cdm.Common;
using Cmd.Business.Character.Business;
using Cmd.Business.Character.ModelsRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public static class CharacterEndpoint
{
    public static void MapPlayerCharacterEndpoint(this WebApplication app)
    {
        var characterCollectionGroup = app.MapGroup("/character").RequireAuthorization();

        // app.MapGet(string.Empty, async (int userId, HttpRequest httpRequest,[FromHeader(Name = "X-GameType")] string gameType, IServiceProvider serviceProvider) =>
        // {
        //     var serviceCharacter = serviceProvider.GetRequiredKeyedService<ICharacterBusiness>(gameType);
        //     serviceCharacter.GetCharacterByCharacterId(userId);
        //
        //     try
        //     {
        //         // var result = serviceCharacter.GetAllCharacterDnd(userId);
        //         //return Results.Ok(result);
        //         return Results.Ok();
        //     }
        //     catch (BusinessException ex)
        //     {
        //         return Results.BadRequest(new { Error = ex.Message });
        //     }
        // });

        app.MapGet($"/characterId", (int characterId, HttpRequest httpRequest, [FromHeader(Name = "X-GameType")] string gameType, IServiceProvider serviceProvider) =>
        {
            var serviceCharacter = serviceProvider.GetRequiredKeyedService<ICharacterBusiness>(gameType);
            serviceCharacter.GetCharacterByCharacterId(characterId);

            try
            {
                 var result = serviceCharacter.GetCharacterByCharacterId(characterId);
                 return Results.Ok(result);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // app.MapPost(string.Empty, async (CharacterDndRequest dndRequest, int userId, HttpRequest httpRequest, [FromServices] CharacterDndBusiness dndBusiness) =>
        //     {
        //         var token = httpRequest.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //
        //         if (string.IsNullOrEmpty(token))
        //             return Results.BadRequest(new { Error = "Le token est manquant." });
        //
        //         try
        //         {
        //             await dndBusiness.CreateCharacterDndRequestAsync(dndRequest, userId);
        //             return Results.Created("/player-character", null);
        //         }
        //         catch (BusinessException ex)
        //         {
        //             return Results.BadRequest(new { Error = ex.Message });
        //         }
        //     });
        //
        // app.MapPut($"playerId", async (CharacterDndRequest dndRequest, int playerId, HttpRequest httpRequest, [FromServices] CharacterDndBusiness dndBusiness) =>
        // {
        //     var token = httpRequest.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //
        //     if (string.IsNullOrEmpty(token))
        //         return Results.BadRequest(new { Error = "Le token est manquant." });
        //
        //     try
        //     {
        //         var result = dndBusiness.UpdateCharacterDndAsync(dndRequest, playerId);
        //         return Results.Ok();
        //     }
        //     catch (BusinessException ex)
        //     {
        //         return Results.BadRequest(new { Error = ex.Message });
        //     }
        // });
    }
    //private IResult GetAllCharacter()
    //{

    //}
}

