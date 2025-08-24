using Cdm.Business.Common.Business.Campaigns;
using Cdm.Business.Common.Models.Campaign.Chapter;
using Cdm.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cdm.ApiService.Endpoints;

public static class ChapterEndpoints
{
    public static void MapChapterEndpoints(this WebApplication app)
    {
        var chapterGroup = app.MapGroup("/api/campaigns/{campaignId:int}/chapters").RequireAuthorization();

        // GET /api/campaigns/{campaignId}/chapters - Get all chapters for a campaign
        chapterGroup.MapGet(string.Empty, async (int campaignId, ChapterBusiness chapterBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var chapters = await chapterBusiness.GetChaptersByCampaignAsync(campaignId, userId);
                return Results.Ok(chapters);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // GET /api/campaigns/{campaignId}/chapters/{id} - Get a specific chapter
        chapterGroup.MapGet("/{id:int}", async (int campaignId, int id, ChapterBusiness chapterBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var chapter = await chapterBusiness.GetChapterByIdAsync(id);
                
                if (chapter == null || chapter.CampaignId != campaignId)
                    return Results.NotFound(new { Error = "Chapter not found" });

                return Results.Ok(chapter);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // GET /api/campaigns/{campaignId}/chapters/{id}/details - Get chapter with full details (ContentBlocks + NPCs)
        chapterGroup.MapGet("/{id:int}/details", async (int campaignId, int id, ChapterBusiness chapterBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var chapterDetail = await chapterBusiness.GetChapterDetailAsync(id, userId);
                
                if (chapterDetail == null || chapterDetail.CampaignId != campaignId)
                    return Results.NotFound(new { Error = "Chapter not found" });

                return Results.Ok(chapterDetail);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // POST /api/campaigns/{campaignId}/chapters - Create a new chapter
        chapterGroup.MapPost(string.Empty, async (int campaignId, [FromBody] CreateChapterRequest createRequest, ChapterBusiness chapterBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                
                // Convert to ChapterRequest with campaignId
                var chapterRequest = new ChapterRequest
                {
                    CampaignId = campaignId,
                    Order = createRequest.Order,
                    Title = createRequest.Title,
                    Content = createRequest.Content,
                    Notes = createRequest.Notes
                };

                var chapter = await chapterBusiness.CreateChapterAsync(chapterRequest, userId);
                return Results.Created($"/api/campaigns/{campaignId}/chapters/{chapter.Id}", chapter);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // PUT /api/campaigns/{campaignId}/chapters/{id} - Update a chapter
        chapterGroup.MapPut("/{id:int}", async (int campaignId, int id, [FromBody] ChapterUpdateRequest updateRequest, ChapterBusiness chapterBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                
                // Verify the chapter belongs to the specified campaign
                var existingChapter = await chapterBusiness.GetChapterByIdAsync(id);
                if (existingChapter == null || existingChapter.CampaignId != campaignId)
                    return Results.NotFound(new { Error = "Chapter not found" });

                var chapter = await chapterBusiness.UpdateChapterAsync(id, updateRequest, userId);
                return Results.Ok(chapter);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // DELETE /api/campaigns/{campaignId}/chapters/{id} - Delete a chapter
        chapterGroup.MapDelete("/{id:int}", async (int campaignId, int id, ChapterBusiness chapterBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                
                // Verify the chapter belongs to the specified campaign
                var existingChapter = await chapterBusiness.GetChapterByIdAsync(id);
                if (existingChapter == null || existingChapter.CampaignId != campaignId)
                    return Results.NotFound(new { Error = "Chapter not found" });

                await chapterBusiness.DeleteChapterAsync(id, userId);
                return Results.NoContent();
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // POST /api/campaigns/{campaignId}/chapters/reorder - Reorder chapters
        chapterGroup.MapPost("/reorder", async (int campaignId, [FromBody] ReorderChaptersRequest reorderRequest, ChapterBusiness chapterBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                await chapterBusiness.ReorderChaptersAsync(campaignId, reorderRequest.ChapterOrderMap, userId);
                return Results.Ok(new { Message = "Chapters reordered successfully" });
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

    // DTOs pour les requests simplifiées
    public record CreateChapterRequest(int Order, string Title, string? Content, string? Notes);
    public record ReorderChaptersRequest(Dictionary<int, int> ChapterOrderMap);
}