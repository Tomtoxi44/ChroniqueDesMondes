using Cdm.Business.Common.Models.Campaign.Campaign;
using Cdm.Common;
using Cdm.Data;
using Cdm.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Cdm.Business.Common.Business.Campaigns;

public class CampaignBusiness
{
    private readonly AppDbContext _dbContext;

    public CampaignBusiness(AppDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    /// <summary>
    /// Creates a new campaign
    /// </summary>
    public async Task<CampaignView> CreateCampaignAsync(CampaignRequest campaignRequest, int createdById)
    {
        var campaign = new Campaign
        {
            Name = campaignRequest.Name,
            Description = campaignRequest.Description,
            GameType = campaignRequest.GameType,
            IsPublic = campaignRequest.IsPublic,
            CreatedById = createdById,
            Settings = campaignRequest.Settings,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        this._dbContext.Campaigns.Add(campaign);
        await this._dbContext.SaveChangesAsync();

        return await this.GetCampaignByIdAsync(campaign.Id) ?? throw new BusinessException("Campaign not found after creation.");
    }

    /// <summary>
    /// Gets a campaign by ID
    /// </summary>
    public async Task<CampaignView?> GetCampaignByIdAsync(int campaignId)
    {
        var campaign = await this._dbContext.Campaigns
            .Include(c => c.CreatedBy)
            .Include(c => c.Chapters.Where(ch => ch.IsActive))
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.IsActive);

        if (campaign == null)
            return null;

        return new CampaignView
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            GameType = campaign.GameType,
            IsPublic = campaign.IsPublic,
            CreatedById = campaign.CreatedById,
            CreatedByName = campaign.CreatedBy.UserName,
            Settings = campaign.Settings,
            CreatedDate = campaign.CreatedDate,
            UpdatedDate = campaign.UpdatedDate,
            IsActive = campaign.IsActive,
            ChapterCount = campaign.Chapters.Count
        };
    }

    /// <summary>
    /// Gets detailed campaign information including chapters
    /// </summary>
    public async Task<CampaignDetailView?> GetCampaignDetailAsync(int campaignId)
    {
        var campaign = await this._dbContext.Campaigns
            .Include(c => c.CreatedBy)
            .Include(c => c.Chapters.Where(ch => ch.IsActive))
            .ThenInclude(ch => ch.ContentBlocks.Where(cb => cb.IsActive))
            .Include(c => c.Chapters)
            .ThenInclude(ch => ch.Characters.Where(ch => ch.IsNpc))
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.IsActive);

        if (campaign == null)
            return null;

        var campaignView = new CampaignDetailView
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            GameType = campaign.GameType,
            IsPublic = campaign.IsPublic,
            CreatedById = campaign.CreatedById,
            CreatedByName = campaign.CreatedBy.UserName,
            Settings = campaign.Settings,
            CreatedDate = campaign.CreatedDate,
            UpdatedDate = campaign.UpdatedDate,
            IsActive = campaign.IsActive,
            ChapterCount = campaign.Chapters.Count,
            Chapters = campaign.Chapters.OrderBy(ch => ch.Order).Select(chapter => new Cdm.Business.Common.Models.Campaign.Chapter.ChapterView
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
            }).ToList()
        };

        return campaignView;
    }

    /// <summary>
    /// Gets campaigns for a specific user
    /// </summary>
    public async Task<List<CampaignView>> GetCampaignsByUserAsync(int userId, bool includePublic = false)
    {
        var query = this._dbContext.Campaigns
            .Include(c => c.CreatedBy)
            .Include(c => c.Chapters.Where(ch => ch.IsActive))
            .Where(c => c.IsActive);

        if (includePublic)
        {
            query = query.Where(c => c.CreatedById == userId || c.IsPublic);
        }
        else
        {
            query = query.Where(c => c.CreatedById == userId);
        }

        var campaigns = await query.OrderByDescending(c => c.CreatedDate).ToListAsync();

        return campaigns.Select(campaign => new CampaignView
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            GameType = campaign.GameType,
            IsPublic = campaign.IsPublic,
            CreatedById = campaign.CreatedById,
            CreatedByName = campaign.CreatedBy.UserName,
            Settings = campaign.Settings,
            CreatedDate = campaign.CreatedDate,
            UpdatedDate = campaign.UpdatedDate,
            IsActive = campaign.IsActive,
            ChapterCount = campaign.Chapters.Count
        }).ToList();
    }

    /// <summary>
    /// Updates an existing campaign
    /// </summary>
    public async Task<CampaignView> UpdateCampaignAsync(int campaignId, CampaignRequest campaignRequest, int userId)
    {
        var campaign = await this._dbContext.Campaigns
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.IsActive);

        if (campaign == null)
        {
            throw new BusinessException("Campaign not found.");
        }

        if (campaign.CreatedById != userId)
        {
            throw new BusinessException("You can only modify campaigns you created.");
        }

        campaign.Name = campaignRequest.Name;
        campaign.Description = campaignRequest.Description;
        campaign.GameType = campaignRequest.GameType;
        campaign.IsPublic = campaignRequest.IsPublic;
        campaign.Settings = campaignRequest.Settings;
        campaign.UpdatedDate = DateTime.UtcNow;

        await this._dbContext.SaveChangesAsync();

        return await this.GetCampaignByIdAsync(campaignId) ?? throw new BusinessException("Campaign not found after update.");
    }

    /// <summary>
    /// Deletes (deactivates) a campaign
    /// </summary>
    public async Task DeleteCampaignAsync(int campaignId, int userId)
    {
        var campaign = await this._dbContext.Campaigns
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.IsActive);

        if (campaign == null)
        {
            throw new BusinessException("Campaign not found.");
        }

        if (campaign.CreatedById != userId)
        {
            throw new BusinessException("You can only delete campaigns you created.");
        }

        campaign.IsActive = false;
        campaign.UpdatedDate = DateTime.UtcNow;

        await this._dbContext.SaveChangesAsync();
    }
}