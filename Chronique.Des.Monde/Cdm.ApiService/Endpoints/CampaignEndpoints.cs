using Cdm.Business.Common.Business.Campaigns;
using Cdm.Business.Common.Models.Campaign;
using Cdm.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cdm.ApiService.Endpoints;

public static class CampaignEndpoints
{
    public static void MapCampaignEndpoints(this WebApplication app)
    {
        var campaignGroup = app.MapGroup("/api/campaigns").RequireAuthorization();

        // GET /api/campaigns - Get all campaigns for authenticated user
        campaignGroup.MapGet(string.Empty, async (CampaignBusiness campaignBusiness, ClaimsPrincipal user, [FromQuery] bool includePublic = false) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var campaigns = await campaignBusiness.GetCampaignsByUserAsync(userId, includePublic);
                return Results.Ok(campaigns);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // GET /api/campaigns/{id} - Get campaign by ID with basic info
        campaignGroup.MapGet("/{id:int}", async (int id, CampaignBusiness campaignBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var campaign = await campaignBusiness.GetCampaignByIdAsync(id);
                
                if (campaign == null)
                    return Results.NotFound(new { Error = "Campaign not found" });

                // Check access permissions
                if (campaign.CreatedById != userId && !campaign.IsPublic)
                    return Results.Forbid();

                return Results.Ok(campaign);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // GET /api/campaigns/{id}/details - Get campaign with full details including chapters
        campaignGroup.MapGet("/{id:int}/details", async (int id, CampaignBusiness campaignBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var campaignDetail = await campaignBusiness.GetCampaignDetailAsync(id);
                
                if (campaignDetail == null)
                    return Results.NotFound(new { Error = "Campaign not found" });

                // Check access permissions
                if (campaignDetail.CreatedById != userId && !campaignDetail.IsPublic)
                    return Results.Forbid();

                return Results.Ok(campaignDetail);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // POST /api/campaigns - Create a new campaign
        campaignGroup.MapPost(string.Empty, async ([FromBody] CampaignRequest campaignRequest, CampaignBusiness campaignBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var campaign = await campaignBusiness.CreateCampaignAsync(campaignRequest, userId);
                return Results.Created($"/api/campaigns/{campaign.Id}", campaign);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // PUT /api/campaigns/{id} - Update an existing campaign
        campaignGroup.MapPut("/{id:int}", async (int id, [FromBody] CampaignRequest campaignRequest, CampaignBusiness campaignBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var campaign = await campaignBusiness.UpdateCampaignAsync(id, campaignRequest, userId);
                return Results.Ok(campaign);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // DELETE /api/campaigns/{id} - Delete a campaign
        campaignGroup.MapDelete("/{id:int}", async (int id, CampaignBusiness campaignBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                await campaignBusiness.DeleteCampaignAsync(id, userId);
                return Results.NoContent();
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