using Cdm.Business.Common.Business.Campaigns;
using Cdm.Business.Common.Models.Campaign.Npc;
using Cdm.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cdm.ApiService.Endpoints;

public static class NpcEndpoints
{
    public static void MapNpcEndpoints(this WebApplication app)
    {
        var npcGroup = app.MapGroup("/api/npcs").RequireAuthorization();

        // POST /api/npcs - Create a new NPC
        npcGroup.MapPost(string.Empty, async ([FromBody] NpcRequest request, NpcBusiness npcBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var npc = await npcBusiness.CreateNpcAsync(request, userId);
                return Results.Created($"/api/npcs/{npc.Id}", npc);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // GET /api/npcs/{id} - Get NPC by ID
        npcGroup.MapGet("/{id:int}", async (int id, NpcBusiness npcBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var npc = await npcBusiness.GetNpcByIdAsync(id, userId);
                
                if (npc == null)
                    return Results.NotFound(new { Error = "NPC not found" });

                return Results.Ok(npc);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // PUT /api/npcs/{id} - Update an NPC
        npcGroup.MapPut("/{id:int}", async (int id, [FromBody] NpcUpdateRequest request, NpcBusiness npcBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var npc = await npcBusiness.UpdateNpcAsync(id, request, userId);
                return Results.Ok(npc);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // DELETE /api/npcs/{id} - Delete an NPC
        npcGroup.MapDelete("/{id:int}", async (int id, NpcBusiness npcBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                await npcBusiness.DeleteNpcAsync(id, userId);
                return Results.NoContent();
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // GET /api/npcs/chapter/{chapterId} - Get all NPCs for a chapter
        npcGroup.MapGet("/chapter/{chapterId:int}", async (int chapterId, NpcBusiness npcBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var npcs = await npcBusiness.GetNpcsByChapterAsync(chapterId, userId);
                return Results.Ok(npcs);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // GET /api/npcs/chapter/{chapterId}/hostile - Get hostile NPCs for combat
        npcGroup.MapGet("/chapter/{chapterId:int}/hostile", async (int chapterId, NpcBusiness npcBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var hostileNpcs = await npcBusiness.GetHostileNpcsByChapterAsync(chapterId, userId);
                return Results.Ok(hostileNpcs);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });
    }

    private static int GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new BusinessException("Invalid user authentication");
        }
        return userId;
    }
}