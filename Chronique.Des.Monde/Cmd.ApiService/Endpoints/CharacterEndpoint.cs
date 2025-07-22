namespace Cmd.ApiService.Endpoints;

using Cmd.Abstraction;
using Cmd.Common;
using Business.Character.Business;
using Business.Character.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public static class CharacterEndpoint
{
    public static void MapPlayerCharacterEndpoint(this WebApplication app)
    {
        app.MapGroup("/character").RequireAuthorization();

        app.MapGet(string.Empty, async (int userId, HttpRequest httpRequest, CharacterDndBusiness dndBusiness) =>
        {
            var token = httpRequest.Headers.Authorization.ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Results.BadRequest(new { Error = "Le token est manquant." });
            }

            try
            {
                var result = dndBusiness.GetAllCharacterDnd(userId);
                return Results.Ok(result);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        app.MapGet($"/characterId", async(int characterId, HttpRequest httpRequest, [FromHeader(Name = "X-GameType")] string gameType, IServiceProvider serviceProvider) =>
        {
            var serviceCharacter = serviceProvider.GetRequiredKeyedService<ICharacterBusiness>(gameType);
            serviceCharacter.GetCharacterByCharacterId(characterId);

            var token = httpRequest.Headers["Authorization"].ToString().Replace(oldValue: "Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Results.BadRequest(new { Error = "Le token est manquant." });
            }

            try
            {
                // var result = business.GetPlayerCharacterByPlayerId(playerId);
                // return Results.Ok(result);
                return Results.Ok();
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        app.MapPost(string.Empty, async (CharacterDndRequest dndRequest, int userId, HttpRequest httpRequest, CharacterDndBusiness dndBusiness) =>
            {
                var token = httpRequest.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return Results.BadRequest(new { Error = "Le token est manquant." });
                }

                try
                {
                    await dndBusiness.CreateCharacterDndRequestAsync(dndRequest, userId);
                    return Results.Created("/player-character", null);
                }
                catch (BusinessException ex)
                {
                    return Results.BadRequest(new { Error = ex.Message });
                }
        });

        app.MapPut($"playerId", async (CharacterDndRequest dndRequest, int playerId, HttpRequest httpRequest, CharacterDndBusiness dndBusiness) =>
        {
            var token = httpRequest.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
                return Results.BadRequest(new { Error = "Le token est manquant." });

            try
            {
                var result = dndBusiness.UpdateCharacterDndAsync(dndRequest, playerId);
                return Results.Ok();
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });
    }
}

