namespace Chronique.Des.Mondes.ApiService.Endpoints;

using Chronique.Des.Mondes.Abstraction;
using Chronique.Des.Mondes.Common;
using Chronique.Des.Mondes.Player.Business;
using Chronique.Des.Mondes.Player.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public static class PlayerCharacterEndpoint
{
    public static void MapPlayerCharacterEndpoint(this WebApplication app)
    {
        app.MapGroup("/player-character").RequireAuthorization();

        app.MapGet(string.Empty, async (int userId, HttpRequest httpRequest, PlayerCharacterBusiness business) =>
        {
            var token = httpRequest.Headers.Authorization.ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Results.BadRequest(new { Error = "Le token est manquant." });
            }

            try
            {
                var result = business.GetAllPlayerCharacter(userId);
                return Results.Ok(result);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        app.MapGet($"/playerId", async(int playerId, HttpRequest httpRequest, [FromHeader(Name = "X-GameType")] string gameType, IServiceProvider serviceProvider) =>
        {
            var servicePlayer = serviceProvider.GetRequiredKeyedService<IPlayerCharacterBusiness>(gameType);
            servicePlayer.GetPlayerCharacterByPlayerId(playerId);

            var token = httpRequest.Headers["Authorization"].ToString().Replace(oldValue: "Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Results.BadRequest(new { Error = "Le token est manquant." });
            }

            try
            {
                var result = business.GetPlayerCharacterByPlayerId(playerId);
                return Results.Ok(result);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        app.MapPost(string.Empty, async (PlayerCharacterRequest request, int userId, HttpRequest httpRequest, PlayerCharacterBusiness business) =>
            {
                var token = httpRequest.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return Results.BadRequest(new { Error = "Le token est manquant." });
                }

                try
                {
                    await business.CreatePlayerCharacterRequestAsync(request, userId);
                    return Results.Created("/player-character", null);
                }
                catch (BusinessException ex)
                {
                    return Results.BadRequest(new { Error = ex.Message });
                }
        });

        app.MapPut($"playerId", async (PlayerCharacterRequest request, int playerId, HttpRequest httpRequest, PlayerCharacterBusiness business) =>
        {
            var token = httpRequest.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
                return Results.BadRequest(new { Error = "Le token est manquant." });

            try
            {
                var result = business.UpdatePlayerCharacterAsync(request, playerId);
                return Results.Ok();
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });
    }
}

