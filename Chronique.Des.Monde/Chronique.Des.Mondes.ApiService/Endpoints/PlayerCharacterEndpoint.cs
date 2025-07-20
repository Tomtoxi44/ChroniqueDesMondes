namespace Chronique.Des.Mondes.ApiService.Endpoints;

using Azure.Core;
using Chronique.Des.Monde.Common;
using Chronique.Des.Monde.Player.Business;
using Chronique.Des.Monde.Player.Models;
using Chronique.Des.Mondes.Data.Models;
using Chronique.Des.Mondes.Web.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

public static class PlayerCharacterEndpoint
{
    public static void MapPlayerCharacterEndpoint(this WebApplication app)
    {
        app.MapGet("/player-character", [Authorize] async (int userId, HttpRequest httpRequest, PlayerCharacterBusiness business) =>
        {
            var token = httpRequest.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Results.BadRequest(new { Error = "Le token est manquant." });
            }

            try
            {
                await business.GetAllPlayerCharacterAsync(userId);
                return Results.Ok();
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        app.MapPost("/player-character", [Authorize] async (PlayerCharacterRequest request, int userId, HttpRequest httpRequest, PlayerCharacterBusiness business) =>
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
    }
}

