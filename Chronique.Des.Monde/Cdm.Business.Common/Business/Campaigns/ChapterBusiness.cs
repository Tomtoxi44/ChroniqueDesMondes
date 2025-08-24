using Cdm.Business.Common.Models.Campaign;
using Cdm.Common;
using Chronique.Des.Mondes.Data;
using Chronique.Des.Mondes.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Cdm.Business.Common.Business.Campaigns;

public class ChapterBusiness
{
    private readonly AppDbContext _dbContext;

    public ChapterBusiness(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Creates a new chapter for a campaign
    /// </summary>
    public async Task<ChapterView> CreateChapterAsync(ChapterRequest chapterRequest, int userId)
    {
        // Verify the campaign exists and user has permission
        var campaign = await _dbContext.Campaigns
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
        var existingChapter = await _dbContext.Chapters
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

        _dbContext.Chapters.Add(chapter);
        await _dbContext.SaveChangesAsync();

        return await GetChapterByIdAsync(chapter.Id) ?? throw new BusinessException("Chapter not found after creation.");
    }

    /// <summary>
    /// Gets a chapter by ID
    /// </summary>
    public async Task<ChapterView?> GetChapterByIdAsync(int chapterId)
    {
        var chapter = await _dbContext.Chapters
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
            Notes = chapter.Notes
        };
    }

    /// <summary>
    /// Gets all chapters for a campaign
    /// </summary>
    public async Task<List<ChapterView>> GetChaptersByCampaignAsync(int campaignId, int userId)
    {
        // Verify the campaign exists and user has access
        var campaign = await _dbContext.Campaigns
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

        var chapters = await _dbContext.Chapters
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
            Notes = chapter.Notes
        }).ToList();
    }

    /// <summary>
    /// Updates an existing chapter
    /// </summary>
    public async Task<ChapterView> UpdateChapterAsync(int chapterId, ChapterUpdateRequest updateRequest, int userId)
    {
        var chapter = await _dbContext.Chapters
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
            var existingChapter = await _dbContext.Chapters
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

        await _dbContext.SaveChangesAsync();

        return await GetChapterByIdAsync(chapterId) ?? throw new BusinessException("Chapter not found after update.");
    }

    /// <summary>
    /// Deletes (deactivates) a chapter
    /// </summary>
    public async Task DeleteChapterAsync(int chapterId, int userId)
    {
        var chapter = await _dbContext.Chapters
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

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Reorders chapters within a campaign
    /// </summary>
    public async Task ReorderChaptersAsync(int campaignId, Dictionary<int, int> chapterOrderMap, int userId)
    {
        var campaign = await _dbContext.Campaigns
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.IsActive);

        if (campaign == null)
        {
            throw new BusinessException("Campaign not found.");
        }

        if (campaign.CreatedById != userId)
        {
            throw new BusinessException("You can only reorder chapters in campaigns you created.");
        }

        var chapters = await _dbContext.Chapters
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

        await _dbContext.SaveChangesAsync();
    }
}