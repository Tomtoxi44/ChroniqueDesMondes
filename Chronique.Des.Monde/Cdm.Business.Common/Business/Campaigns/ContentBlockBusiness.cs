using Cdm.Business.Common.Models.Campaign.ContentBlock;
using Cdm.Common;
using Chronique.Des.Mondes.Data;
using Chronique.Des.Mondes.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Cdm.Business.Common.Business.Campaigns;

public class ContentBlockBusiness
{
    private readonly AppDbContext _context;

    public ContentBlockBusiness(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ContentBlockView> CreateContentBlockAsync(ContentBlockRequest request, int userId)
    {
        // Vérifier que l'utilisateur a accès au chapitre
        var chapter = await _context.Chapters
            .Include(c => c.Campaign)
            .FirstOrDefaultAsync(c => c.Id == request.ChapterId);

        if (chapter == null)
            throw new BusinessException("Chapter not found");

        if (chapter.Campaign.CreatedById != userId)
            throw new BusinessException("You don't have permission to add content blocks to this chapter");

        // Valider le NPC si c'est un dialogue
        if (request.Type == ContentBlockTypes.NpcDialogue)
        {
            if (request.CharacterId == null || string.IsNullOrEmpty(request.NpcMood))
                throw new BusinessException("CharacterId and NpcMood are required for NpcDialogue blocks");

            var character = await _context.CharactersDnd
                .FirstOrDefaultAsync(c => c.Id == request.CharacterId && c.IsNpc && c.ChapterId == request.ChapterId);

            if (character == null)
                throw new BusinessException("Character not found or is not an NPC in this chapter");
        }

        // Vérifier que l'ordre n'est pas déjà utilisé
        var existingBlock = await _context.ContentBlocks
            .FirstOrDefaultAsync(cb => cb.ChapterId == request.ChapterId && cb.Order == request.Order);

        if (existingBlock != null)
            throw new BusinessException($"A content block with order {request.Order} already exists in this chapter");

        var contentBlock = new ContentBlock
        {
            ChapterId = request.ChapterId,
            Order = request.Order,
            Type = request.Type,
            Title = request.Title,
            Content = request.Content,
            CharacterId = request.CharacterId,
            NpcMood = request.NpcMood,
            Tags = request.Tags != null ? JsonSerializer.Serialize(request.Tags) : null,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _context.ContentBlocks.Add(contentBlock);
        await _context.SaveChangesAsync();

        return await GetContentBlockViewAsync(contentBlock.Id);
    }

    public async Task<ContentBlockView> UpdateContentBlockAsync(int id, ContentBlockUpdateRequest request, int userId)
    {
        var contentBlock = await _context.ContentBlocks
            .Include(cb => cb.Chapter)
            .ThenInclude(c => c.Campaign)
            .FirstOrDefaultAsync(cb => cb.Id == id);

        if (contentBlock == null)
            throw new BusinessException("Content block not found");

        if (contentBlock.Chapter.Campaign.CreatedById != userId)
            throw new BusinessException("You don't have permission to update this content block");

        // Mettre à jour les propriétés
        if (!string.IsNullOrEmpty(request.Title))
            contentBlock.Title = request.Title;

        if (!string.IsNullOrEmpty(request.Content))
            contentBlock.Content = request.Content;

        if (request.Order.HasValue)
        {
            // Vérifier que le nouvel ordre n'est pas déjà utilisé
            var existingBlock = await _context.ContentBlocks
                .FirstOrDefaultAsync(cb => cb.ChapterId == contentBlock.ChapterId && 
                                         cb.Order == request.Order.Value && 
                                         cb.Id != id);

            if (existingBlock != null)
                throw new BusinessException($"A content block with order {request.Order} already exists in this chapter");

            contentBlock.Order = request.Order.Value;
        }

        if (request.CharacterId.HasValue)
        {
            if (contentBlock.Type == ContentBlockTypes.NpcDialogue)
            {
                var character = await _context.CharactersDnd
                    .FirstOrDefaultAsync(c => c.Id == request.CharacterId && c.IsNpc && c.ChapterId == contentBlock.ChapterId);

                if (character == null)
                    throw new BusinessException("Character not found or is not an NPC in this chapter");
            }
            
            contentBlock.CharacterId = request.CharacterId;
        }

        if (!string.IsNullOrEmpty(request.NpcMood))
            contentBlock.NpcMood = request.NpcMood;

        if (request.Tags != null)
            contentBlock.Tags = JsonSerializer.Serialize(request.Tags);

        if (request.IsActive.HasValue)
            contentBlock.IsActive = request.IsActive.Value;

        contentBlock.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetContentBlockViewAsync(id);
    }

    public async Task DeleteContentBlockAsync(int id, int userId)
    {
        var contentBlock = await _context.ContentBlocks
            .Include(cb => cb.Chapter)
            .ThenInclude(c => c.Campaign)
            .FirstOrDefaultAsync(cb => cb.Id == id);

        if (contentBlock == null)
            throw new BusinessException("Content block not found");

        if (contentBlock.Chapter.Campaign.CreatedById != userId)
            throw new BusinessException("You don't have permission to delete this content block");

        _context.ContentBlocks.Remove(contentBlock);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ContentBlockView>> GetContentBlocksByChapterAsync(int chapterId, int userId)
    {
        var chapter = await _context.Chapters
            .Include(c => c.Campaign)
            .FirstOrDefaultAsync(c => c.Id == chapterId);

        if (chapter == null)
            throw new BusinessException("Chapter not found");

        // Vérifier l'accès (créateur ou campagne publique)
        if (chapter.Campaign.CreatedById != userId && !chapter.Campaign.IsPublic)
            throw new BusinessException("You don't have permission to view this chapter's content blocks");

        var contentBlocks = await _context.ContentBlocks
            .Include(cb => cb.Character)
            .Where(cb => cb.ChapterId == chapterId && cb.IsActive)
            .OrderBy(cb => cb.Order)
            .ToListAsync();

        return contentBlocks.Select(MapToContentBlockView).ToList();
    }

    public async Task<ContentBlockView> GetContentBlockByIdAsync(int id, int userId)
    {
        var contentBlock = await _context.ContentBlocks
            .Include(cb => cb.Chapter)
            .ThenInclude(c => c.Campaign)
            .Include(cb => cb.Character)
            .FirstOrDefaultAsync(cb => cb.Id == id);

        if (contentBlock == null)
            throw new BusinessException("Content block not found");

        // Vérifier l'accès
        if (contentBlock.Chapter.Campaign.CreatedById != userId && !contentBlock.Chapter.Campaign.IsPublic)
            throw new BusinessException("You don't have permission to view this content block");

        return MapToContentBlockView(contentBlock);
    }

    public async Task ReorderContentBlocksAsync(int chapterId, Dictionary<int, int> orderMap, int userId)
    {
        var chapter = await _context.Chapters
            .Include(c => c.Campaign)
            .FirstOrDefaultAsync(c => c.Id == chapterId);

        if (chapter == null)
            throw new BusinessException("Chapter not found");

        if (chapter.Campaign.CreatedById != userId)
            throw new BusinessException("You don't have permission to reorder content blocks in this chapter");

        var contentBlocks = await _context.ContentBlocks
            .Where(cb => cb.ChapterId == chapterId && orderMap.Keys.Contains(cb.Id))
            .ToListAsync();

        foreach (var contentBlock in contentBlocks)
        {
            if (orderMap.TryGetValue(contentBlock.Id, out var newOrder))
            {
                contentBlock.Order = newOrder;
                contentBlock.UpdatedDate = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task<ContentBlockView> GetContentBlockViewAsync(int id)
    {
        var contentBlock = await _context.ContentBlocks
            .Include(cb => cb.Character)
            .FirstOrDefaultAsync(cb => cb.Id == id);

        return MapToContentBlockView(contentBlock!);
    }

    private static ContentBlockView MapToContentBlockView(ContentBlock contentBlock)
    {
        var tags = new List<string>();
        if (!string.IsNullOrEmpty(contentBlock.Tags))
        {
            try
            {
                tags = JsonSerializer.Deserialize<List<string>>(contentBlock.Tags) ?? new List<string>();
            }
            catch
            {
                // Si la désérialisation échoue, on garde une liste vide
            }
        }

        return new ContentBlockView
        {
            Id = contentBlock.Id,
            ChapterId = contentBlock.ChapterId,
            Order = contentBlock.Order,
            Type = contentBlock.Type,
            Title = contentBlock.Title,
            Content = contentBlock.Content,
            CharacterId = contentBlock.CharacterId,
            CharacterName = contentBlock.Character?.Name,
            NpcMood = contentBlock.NpcMood,
            Tags = tags,
            IsActive = contentBlock.IsActive,
            CreatedDate = contentBlock.CreatedDate,
            UpdatedDate = contentBlock.UpdatedDate
        };
    }
}