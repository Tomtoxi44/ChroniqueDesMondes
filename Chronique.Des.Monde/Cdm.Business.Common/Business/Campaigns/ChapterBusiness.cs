using Cdm.Business.Common.Models.Campaign.Chapter;
using Cdm.Business.Common.Models.Campaign.ContentBlock;
using Cdm.Business.Common.Models.Campaign.Npc;
using Cdm.Common;
using Cdm.Data.Dnd;
using Cdm.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Cdm.Business.Common.Business.Campaigns;

public class ChapterBusiness
{
    private readonly DndDbContext dbContext;

    public ChapterBusiness(DndDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Creates a new chapter for a campaign
    /// </summary>
    public async Task<ChapterView> CreateChapterAsync(ChapterRequest chapterRequest, int userId)
    {
        // Verify the campaign exists and user has permission
        var campaign = await this.dbContext.Campaigns
            .FirstOrDefaultAsync(c => c.Id == chapterRequest.CampaignId && c.IsActive);

        if (campaign == null)
        {
            throw new BusinessException("Campaign not found.");
        }

        if (campaign.CreatedById != userId)
        {
            throw new BusinessException("You can only add chapters to campaigns you created.");
        }

        // Verify order uniqueness within the campaign
        var existingChapter = await this.dbContext.Chapters
            .FirstOrDefaultAsync(ch => ch.CampaignId == chapterRequest.CampaignId && 
                                      ch.Order == chapterRequest.Order && 
                                      ch.IsActive);

        if (existingChapter != null)
        {
            throw new BusinessException($"A chapter with order {chapterRequest.Order} already exists in this campaign.");
        }

        var chapter = new Chapter
        {
            CampaignId = chapterRequest.CampaignId,
            Order = chapterRequest.Order,
            Title = chapterRequest.Title,
            Content = chapterRequest.Content,
            Notes = chapterRequest.Notes,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        this.dbContext.Chapters.Add(chapter);
        await this.dbContext.SaveChangesAsync();

        return await this.GetChapterByIdAsync(chapter.Id) ?? throw new BusinessException("Chapter not found after creation.");
    }

    /// <summary>
    /// Gets a chapter by ID
    /// </summary>
    public async Task<ChapterView?> GetChapterByIdAsync(int chapterId)
    {
        var chapter = await this.dbContext.Chapters
            .Include(ch => ch.ContentBlocks)
            .Include(ch => ch.Characters.Where(c => c.IsNpc))
            .FirstOrDefaultAsync(ch => ch.Id == chapterId && ch.IsActive);

        if (chapter == null)
            return null;

        return new ChapterView
        {
            Id = chapter.Id,
            CampaignId = chapter.CampaignId,
            Order = chapter.Order,
            Title = chapter.Title,
            Content = chapter.Content,
            IsActive = chapter.IsActive,
            CreatedDate = chapter.CreatedDate,
            UpdatedDate = chapter.UpdatedDate,
            Notes = chapter.Notes,
            ContentBlocksCount = chapter.ContentBlocks.Count(cb => cb.IsActive),
            CharactersCount = chapter.Characters.Count(c => c.IsNpc),
            HostileCharactersCount = chapter.Characters.Count(c => c.IsNpc && c.IsHostile)
        };
    }

    /// <summary>
    /// Gets detailed chapter information including content blocks and NPCs
    /// </summary>
    public async Task<ChapterDetailView?> GetChapterDetailAsync(int chapterId, int userId)
    {
        var chapter = await this.dbContext.Chapters
            .Include(ch => ch.Campaign)
            .Include(ch => ch.ContentBlocks.Where(cb => cb.IsActive))
            .ThenInclude(cb => cb.Character)
            .Include(ch => ch.Characters.Where(c => c.IsNpc))
            .FirstOrDefaultAsync(ch => ch.Id == chapterId && ch.IsActive);

        if (chapter == null)
            return null;

        // Check access permissions
        if (chapter.Campaign.CreatedById != userId && !chapter.Campaign.IsPublic)
            throw new BusinessException("You don't have access to this chapter.");

        return new ChapterDetailView
        {
            Id = chapter.Id,
            CampaignId = chapter.CampaignId,
            Order = chapter.Order,
            Title = chapter.Title,
            Content = chapter.Content,
            IsActive = chapter.IsActive,
            CreatedDate = chapter.CreatedDate,
            UpdatedDate = chapter.UpdatedDate,
            Notes = chapter.Notes,
            ContentBlocksCount = chapter.ContentBlocks.Count,
            CharactersCount = chapter.Characters.Count,
            HostileCharactersCount = chapter.Characters.Count(c => c.IsHostile),
            ContentBlocks = chapter.ContentBlocks.OrderBy(cb => cb.Order).Select(this.MapToContentBlockView).ToList(),
            Npcs = chapter.Characters.Select(this.MapToNpcView).ToList()
        };
    }

    /// <summary>
    /// Gets all chapters for a campaign
    /// </summary>
    public async Task<List<ChapterView>> GetChaptersByCampaignAsync(int campaignId, int userId)
    {
        // Verify the campaign exists and user has access
        var campaign = await this.dbContext.Campaigns
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.IsActive);

        if (campaign == null)
        {
            throw new BusinessException("Campaign not found.");
        }

        // Check if user has access (owner or public campaign)
        if (campaign.CreatedById != userId && !campaign.IsPublic)
        {
            throw new BusinessException("You don't have access to this campaign.");
        }

        var chapters = await this.dbContext.Chapters
            .Include(ch => ch.ContentBlocks.Where(cb => cb.IsActive))
            .Include(ch => ch.Characters.Where(c => c.IsNpc))
            .Where(ch => ch.CampaignId == campaignId && ch.IsActive)
            .OrderBy(ch => ch.Order)
            .ToListAsync();

        return chapters.Select(chapter => new ChapterView
        {
            Id = chapter.Id,
            CampaignId = chapter.CampaignId,
            Order = chapter.Order,
            Title = chapter.Title,
            Content = chapter.Content,
            IsActive = chapter.IsActive,
            CreatedDate = chapter.CreatedDate,
            UpdatedDate = chapter.UpdatedDate,
            Notes = chapter.Notes,
            ContentBlocksCount = chapter.ContentBlocks.Count,
            CharactersCount = chapter.Characters.Count,
            HostileCharactersCount = chapter.Characters.Count(c => c.IsHostile)
        }).ToList();
    }

    /// <summary>
    /// Updates an existing chapter
    /// </summary>
    public async Task<ChapterView> UpdateChapterAsync(int chapterId, ChapterUpdateRequest updateRequest, int userId)
    {
        var chapter = await this.dbContext.Chapters
            .Include(ch => ch.Campaign)
            .FirstOrDefaultAsync(ch => ch.Id == chapterId && ch.IsActive);

        if (chapter == null)
        {
            throw new BusinessException("Chapter not found.");
        }

        if (chapter.Campaign.CreatedById != userId)
        {
            throw new BusinessException("You can only modify chapters in campaigns you created.");
        }

        // Check order uniqueness if order is being changed
        if (updateRequest.Order.HasValue && updateRequest.Order.Value != chapter.Order)
        {
            var existingChapter = await this.dbContext.Chapters
                .FirstOrDefaultAsync(ch => ch.CampaignId == chapter.CampaignId && 
                                          ch.Order == updateRequest.Order.Value && 
                                          ch.Id != chapterId && 
                                          ch.IsActive);

            if (existingChapter != null)
            {
                throw new BusinessException($"A chapter with order {updateRequest.Order.Value} already exists in this campaign.");
            }

            chapter.Order = updateRequest.Order.Value;
        }

        if (!string.IsNullOrEmpty(updateRequest.Title))
        {
            chapter.Title = updateRequest.Title;
        }

        if (updateRequest.Content != null)
        {
            chapter.Content = updateRequest.Content;
        }

        if (updateRequest.IsActive.HasValue)
        {
            chapter.IsActive = updateRequest.IsActive.Value;
        }

        if (updateRequest.Notes != null)
        {
            chapter.Notes = updateRequest.Notes;
        }

        chapter.UpdatedDate = DateTime.UtcNow;

        await this.dbContext.SaveChangesAsync();

        return await this.GetChapterByIdAsync(chapterId) ?? throw new BusinessException("Chapter not found after update.");
    }

    /// <summary>
    /// Deletes (deactivates) a chapter
    /// </summary>
    public async Task DeleteChapterAsync(int chapterId, int userId)
    {
        var chapter = await this.dbContext.Chapters
            .Include(ch => ch.Campaign)
            .FirstOrDefaultAsync(ch => ch.Id == chapterId && ch.IsActive);

        if (chapter == null)
        {
            throw new BusinessException("Chapter not found.");
        }

        if (chapter.Campaign.CreatedById != userId)
        {
            throw new BusinessException("You can only delete chapters in campaigns you created.");
        }

        chapter.IsActive = false;
        chapter.UpdatedDate = DateTime.UtcNow;

        await this.dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Reorders chapters within a campaign
    /// </summary>
    public async Task ReorderChaptersAsync(int campaignId, Dictionary<int, int> chapterOrderMap, int userId)
    {
        var campaign = await this.dbContext.Campaigns
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.IsActive);

        if (campaign == null)
        {
            throw new BusinessException("Campaign not found.");
        }

        if (campaign.CreatedById != userId)
        {
            throw new BusinessException("You can only reorder chapters in campaigns you created.");
        }

        var chapters = await this.dbContext.Chapters
            .Where(ch => ch.CampaignId == campaignId && ch.IsActive)
            .ToListAsync();

        foreach (var chapter in chapters)
        {
            if (chapterOrderMap.TryGetValue(chapter.Id, out var newOrder))
            {
                chapter.Order = newOrder;
                chapter.UpdatedDate = DateTime.UtcNow;
            }
        }

        await this.dbContext.SaveChangesAsync();
    }

    private ContentBlockView MapToContentBlockView(ContentBlock contentBlock)
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

    private NpcView MapToNpcView(ACharacter character)
    {
        var tags = new List<string>();
        if (!string.IsNullOrEmpty(character.Tags))
        {
            try
            {
                tags = JsonSerializer.Deserialize<List<string>>(character.Tags) ?? new List<string>();
            }
            catch
            {
                // Si la désérialisation échoue, on garde une liste vide
            }
        }

        return new NpcView
        {
            Id = character.Id,
            ChapterId = character.ChapterId ?? 0,
            Name = character.Name,
            Description = character.Background,
            GameType = character.GameType,
            IsHostile = character.IsHostile,
            Tags = tags,
            IsSystemCharacter = character.IsSystemCharacter,
            CreatedAt = character.CreatedAt,
            DialogueBlocksCount = character.ContentBlocks?.Count ?? 0
        };
    }
}