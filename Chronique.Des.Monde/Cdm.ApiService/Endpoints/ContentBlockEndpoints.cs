using Cdm.Business.Common.Business.Campaigns;
using Cdm.Business.Common.Models.Campaign.ContentBlock;
using Cdm.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cdm.ApiService.Endpoints;

public static class ContentBlockEndpoints
{
    public static void MapContentBlockEndpoints(this WebApplication app)
    {
        var contentBlockGroup = app.MapGroup("/api/content-blocks").RequireAuthorization();

        // POST /api/content-blocks - Create a new content block
        contentBlockGroup.MapPost(string.Empty, async ([FromBody] ContentBlockRequest request, ContentBlockBusiness contentBlockBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var contentBlock = await contentBlockBusiness.CreateContentBlockAsync(request, userId);
                return Results.Created($"/api/content-blocks/{contentBlock.Id}", contentBlock);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // GET /api/content-blocks/{id} - Get content block by ID
        contentBlockGroup.MapGet("/{id:int}", async (int id, ContentBlockBusiness contentBlockBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var contentBlock = await contentBlockBusiness.GetContentBlockByIdAsync(id, userId);
                return Results.Ok(contentBlock);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // PUT /api/content-blocks/{id} - Update a content block
        contentBlockGroup.MapPut("/{id:int}", async (int id, [FromBody] ContentBlockUpdateRequest request, ContentBlockBusiness contentBlockBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var contentBlock = await contentBlockBusiness.UpdateContentBlockAsync(id, request, userId);
                return Results.Ok(contentBlock);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // DELETE /api/content-blocks/{id} - Delete a content block
        contentBlockGroup.MapDelete("/{id:int}", async (int id, ContentBlockBusiness contentBlockBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                await contentBlockBusiness.DeleteContentBlockAsync(id, userId);
                return Results.NoContent();
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // GET /api/content-blocks/chapter/{chapterId} - Get all content blocks for a chapter
        contentBlockGroup.MapGet("/chapter/{chapterId:int}", async (int chapterId, ContentBlockBusiness contentBlockBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                var contentBlocks = await contentBlockBusiness.GetContentBlocksByChapterAsync(chapterId, userId);
                return Results.Ok(contentBlocks);
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // POST /api/content-blocks/chapter/{chapterId}/reorder - Reorder content blocks in a chapter
        contentBlockGroup.MapPost("/chapter/{chapterId:int}/reorder", async (int chapterId, [FromBody] ReorderContentBlocksRequest request, ContentBlockBusiness contentBlockBusiness, ClaimsPrincipal user) =>
        {
            try
            {
                var userId = GetUserIdFromClaims(user);
                await contentBlockBusiness.ReorderContentBlocksAsync(chapterId, request.OrderMap, userId);
                return Results.Ok(new { Message = "Content blocks reordered successfully" });
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // GET /api/content-blocks/types - Get available content block types
        contentBlockGroup.MapGet("/types", () =>
        {
            return Results.Ok(new
            {
                Types = ContentBlockTypes.All,
                Moods = NpcMoods.All
            });
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

    // DTO for reordering
    public record ReorderContentBlocksRequest(Dictionary<int, int> OrderMap);
}