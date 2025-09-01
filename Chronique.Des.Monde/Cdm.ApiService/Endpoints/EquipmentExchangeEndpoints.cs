using Microsoft.AspNetCore.Mvc;
using Cmd.Abstraction.Equipment;
using Cdm.Data.Common.Models;
using System.Security.Claims;

namespace Cdm.ApiService.Endpoints;

/// <summary>
/// Endpoints pour la gestion des échanges d'équipements (propositions MJ et échanges joueurs)
/// </summary>
public static class EquipmentExchangeEndpoints
{
    public static void MapEquipmentExchangeEndpoints(this WebApplication app)
    {
        var exchangeGroup = app.MapGroup("/api/equipment/exchange").RequireAuthorization();

        // === PROPOSITIONS MJ → JOUEUR ===

        // POST /api/equipment/exchange/offer - MJ propose un équipement à un joueur
        exchangeGroup.MapPost("/offer", async (
            [FromBody] CreateOfferRequest request,
            IEquipmentExchangeService exchangeService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);

                // Vérifier que l'utilisateur est bien dans la campagne
                if (!await exchangeService.IsPlayerInCampaignAsync(request.CampaignId, userId))
                {
                    return Results.Forbid();
                }

                var offerId = await exchangeService.CreateOfferAsync(
                    request.CampaignId, 
                    userId, // Le MJ qui fait l'offre
                    request.TargetPlayerId, 
                    request.EquipmentId, 
                    request.Quantity, 
                    request.Message);

                return Results.Created($"/api/equipment/exchange/offer/{offerId}", new { id = offerId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/equipment/exchange/offers/received?campaignId={id} - Propositions reçues par un joueur
        exchangeGroup.MapGet("/offers/received", async (
            [FromQuery] int campaignId,
            IEquipmentExchangeService exchangeService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);

                if (!await exchangeService.IsPlayerInCampaignAsync(campaignId, userId))
                {
                    return Results.Forbid();
                }

                var offers = await exchangeService.GetPlayerOffersAsync(campaignId, userId);
                return Results.Ok(offers);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/equipment/exchange/offers/sent?campaignId={id} - Propositions envoyées par un MJ
        exchangeGroup.MapGet("/offers/sent", async (
            [FromQuery] int campaignId,
            IEquipmentExchangeService exchangeService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);

                if (!await exchangeService.IsPlayerInCampaignAsync(campaignId, userId))
                {
                    return Results.Forbid();
                }

                var offers = await exchangeService.GetGmOffersAsync(campaignId, userId);
                return Results.Ok(offers);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT /api/equipment/exchange/offer/{id}/respond - Joueur répond à une proposition
        exchangeGroup.MapPut("/offer/{id:int}/respond", async (
            int id,
            [FromBody] RespondToOfferRequest request,
            IEquipmentExchangeService exchangeService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);

                var success = await exchangeService.RespondToOfferAsync(id, userId, request.Accepted, request.ResponseMessage);
                
                if (!success)
                {
                    return Results.NotFound();
                }

                return Results.Ok(new { message = request.Accepted ? "Offer accepted" : "Offer declined" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // === ÉCHANGES JOUEUR ↔ JOUEUR ===

        // POST /api/equipment/exchange/trade - Proposer un échange entre joueurs
        exchangeGroup.MapPost("/trade", async (
            [FromBody] CreateTradeRequest request,
            IEquipmentExchangeService exchangeService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);

                // Vérifier que les deux joueurs sont dans la même campagne
                if (!await exchangeService.ArePlayersInSameCampaignAsync(request.CampaignId, userId, request.ToPlayerId))
                {
                    return Results.Forbid();
                }

                // Vérifier que le joueur a assez d'exemplaires
                if (!await exchangeService.HasSufficientQuantityAsync(request.FromCharacterId, request.EquipmentId, request.Quantity))
                {
                    return Results.BadRequest(new { error = "Insufficient quantity in inventory" });
                }

                var tradeId = await exchangeService.ProposeTradeAsync(
                    request.CampaignId,
                    userId,
                    request.ToPlayerId,
                    request.EquipmentId,
                    request.Quantity,
                    request.Message);

                return Results.Created($"/api/equipment/exchange/trade/{tradeId}", new { id = tradeId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // GET /api/equipment/exchange/trades?campaignId={id} - Échanges d'un joueur
        exchangeGroup.MapGet("/trades", async (
            [FromQuery] int campaignId,
            IEquipmentExchangeService exchangeService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);

                if (!await exchangeService.IsPlayerInCampaignAsync(campaignId, userId))
                {
                    return Results.Forbid();
                }

                var trades = await exchangeService.GetPlayerTradesAsync(campaignId, userId);
                return Results.Ok(trades);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT /api/equipment/exchange/trade/{id}/respond - Répondre à un échange
        exchangeGroup.MapPut("/trade/{id:int}/respond", async (
            int id,
            [FromBody] RespondToTradeRequest request,
            IEquipmentExchangeService exchangeService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);

                var success = await exchangeService.RespondToTradeAsync(id, userId, request.Accepted);
                
                if (!success)
                {
                    return Results.NotFound();
                }

                return Results.Ok(new { message = request.Accepted ? "Trade accepted" : "Trade declined" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // === INVENTAIRE ===

        // GET /api/equipment/exchange/inventory/{characterId} - Inventaire d'un personnage
        exchangeGroup.MapGet("/inventory/{characterId:int}", async (
            int characterId,
            IEquipmentExchangeService exchangeService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                var inventory = await exchangeService.GetCharacterInventoryAsync(characterId);
                return Results.Ok(inventory);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // POST /api/equipment/exchange/inventory/{characterId}/add - Ajouter un équipement à l'inventaire
        exchangeGroup.MapPost("/inventory/{characterId:int}/add", async (
            int characterId,
            [FromBody] AddToInventoryRequest request,
            IEquipmentExchangeService exchangeService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                var equipmentId = request.EquipmentId;
                var quantity = request?.Quantity ?? 1;
                var notes = request?.Notes;

                // TODO: Implémenter l'ajout à l'inventaire
                var inventoryId = 1; // await exchangeService.AddEquipmentToInventoryAsync(characterId, equipmentId, quantity, notes);
                return Results.Created($"/api/character/{characterId}/inventory/{inventoryId}", new { id = inventoryId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        // PUT /api/equipment/exchange/inventory/{characterId}/equip - Équiper/déséquiper un objet
        exchangeGroup.MapPut("/inventory/{characterId:int}/equip", async (
            int characterId,
            [FromBody] EquipItemRequest request,
            IEquipmentExchangeService exchangeService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                // TODO: Vérifier que l'utilisateur possède ce personnage

                var success = await exchangeService.EquipItemAsync(characterId, request.EquipmentId, request.Equipped);
                
                if (!success)
                {
                    return Results.NotFound();
                }

                return Results.Ok(new { message = request.Equipped ? "Item equipped" : "Item unequipped" });
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

// === MODÈLES DE REQUÊTE ===

public record CreateOfferRequest(int CampaignId, int TargetPlayerId, int EquipmentId, int Quantity, string? Message);
public record RespondToOfferRequest(bool Accepted, string? ResponseMessage);
public record CreateTradeRequest(int CampaignId, int ToPlayerId, int EquipmentId, int Quantity, string? Message);
public record RespondToTradeRequest(bool Accepted);
public record EquipItemRequest(bool Equipped);