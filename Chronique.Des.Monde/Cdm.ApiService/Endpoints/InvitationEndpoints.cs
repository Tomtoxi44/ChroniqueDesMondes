using Cdm.Business.Common.Business.Campaigns;
using Cdm.Business.Common.Models.Campaign.Invitation;
using Cdm.Common;
using Microsoft.AspNetCore.Mvc;

namespace Cdm.ApiService.Endpoints;

public static class InvitationEndpoints
{
    public static void MapInvitationEndpoints(this WebApplication app)
    {
        var invitationGroup = app.MapGroup("/api/invitations").RequireAuthorization();

        // POST /api/campaigns/{campaignId}/invite - Envoyer une invitation
        app.MapPost("/api/campaigns/{campaignId:int}/invite", 
            async (int campaignId, [FromBody] InvitationRequest request, InvitationService invitationService, HttpContext context) =>
        {
            try
            {
                var userId = GetUserIdFromContext(context);
                var invitation = await invitationService.SendInvitationAsync(campaignId, request, userId);
                return Results.Created($"/api/invitations/{invitation.Id}", invitation);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        }).RequireAuthorization();

        // GET /api/campaigns/{campaignId}/members - Récupérer les membres d'une campagne
        app.MapGet("/api/campaigns/{campaignId:int}/members", 
            async (int campaignId, InvitationService invitationService, HttpContext context) =>
        {
            try
            {
                var userId = GetUserIdFromContext(context);
                var members = await invitationService.GetCampaignMembersAsync(campaignId, userId);
                return Results.Ok(members);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        }).RequireAuthorization();

        // PUT /api/invitations/{token}/respond - Répondre à une invitation
        invitationGroup.MapPut("/{token}/respond", 
            async (string token, [FromBody] InvitationResponse response, InvitationService invitationService, HttpContext context) =>
        {
            try
            {
                var userId = GetUserIdFromContext(context);
                var accepted = await invitationService.RespondToInvitationAsync(token, response, userId);
                return Results.Ok(new { Accepted = accepted });
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // DELETE /api/campaigns/{campaignId}/participants/{participantId} - Retirer un participant
        app.MapDelete("/api/campaigns/{campaignId:int}/participants/{participantId:int}", 
            async (int campaignId, int participantId, InvitationService invitationService, HttpContext context) =>
        {
            try
            {
                var userId = GetUserIdFromContext(context);
                await invitationService.RemoveParticipantAsync(campaignId, participantId, userId);
                return Results.NoContent();
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        }).RequireAuthorization();

        // DELETE /api/invitations/{invitationId} - Annuler une invitation
        invitationGroup.MapDelete("/{invitationId:int}", 
            async (int invitationId, InvitationService invitationService, HttpContext context) =>
        {
            try
            {
                var userId = GetUserIdFromContext(context);
                await invitationService.CancelInvitationAsync(invitationId, userId);
                return Results.NoContent();
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });
    }

    private static int GetUserIdFromContext(HttpContext context)
    {
        var userIdClaim = context.User.FindFirst("sub") ?? context.User.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new BusinessException("User not authenticated");
        }
        return userId;
    }
}