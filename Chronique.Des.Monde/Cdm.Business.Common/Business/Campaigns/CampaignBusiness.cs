using Cdm.Business.Common.Models.Campaign;
using Cdm.Common;
using Cdm.Common.Enums;
using Chronique.Des.Mondes.Data;
using Chronique.Des.Mondes.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Cdm.Business.Common.Business.Campaigns;

public class CampaignBusiness
{
    private readonly AppDbContext _dbContext;

    public CampaignBusiness(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Creates a new campaign
    /// </summary>
    public async Task<CampaignView> CreateCampaignAsync(CampaignRequest campaignRequest, int createdById)
    {
        // Validate name uniqueness for the user
        var existingCampaign = await _dbContext.Campaigns
            .FirstOrDefaultAsync(c => c.Name == campaignRequest.Name && c.CreatedById == createdById);

        if (existingCampaign != null)
        {
            throw new BusinessException($"A campaign with the name '{campaignRequest.Name}' already exists for this user.");
        }

        var campaign = new Campaign
        {
            Name = campaignRequest.Name,
            Description = campaignRequest.Description,
            GameType = (int)campaignRequest.GameType,
            IsPublic = campaignRequest.IsPublic,
            CreatedById = createdById,
            Settings = campaignRequest.Settings,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        _dbContext.Campaigns.Add(campaign);
        await _dbContext.SaveChangesAsync();

        return await GetCampaignByIdAsync(campaign.Id) ?? throw new BusinessException("Campaign not found after creation.");
    }

    /// <summary>
    /// Gets a campaign by ID with creator information
    /// </summary>
    public async Task<CampaignView?> GetCampaignByIdAsync(int campaignId)
    {
        var campaign = await _dbContext.Campaigns
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
            GameType = (GameType)campaign.GameType,
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
        var campaign = await _dbContext.Campaigns
            .Include(c => c.CreatedBy)
            .Include(c => c.Chapters.Where(ch => ch.IsActive))
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.IsActive);

        if (campaign == null)
            return null;

        return new CampaignDetailView
        {
            Id = campaign.Id,
            Name = campaign.Name,
            Description = campaign.Description,
            GameType = (GameType)campaign.GameType,
            IsPublic = campaign.IsPublic,
            CreatedById = campaign.CreatedById,
            CreatedByName = campaign.CreatedBy.UserName,
            Settings = campaign.Settings,
            CreatedDate = campaign.CreatedDate,
            UpdatedDate = campaign.UpdatedDate,
            IsActive = campaign.IsActive,
            ChapterCount = campaign.Chapters.Count,
            Chapters = campaign.Chapters.OrderBy(ch => ch.Order).Select(ch => new ChapterView
            {
                Id = ch.Id,
                CampaignId = ch.CampaignId,
                Order = ch.Order,
                Title = ch.Title,
                Content = ch.Content,
                IsActive = ch.IsActive,
                CreatedDate = ch.CreatedDate,
                UpdatedDate = ch.UpdatedDate,
                Notes = ch.Notes
            }).ToList()
        };
    }

    /// <summary>
    /// Gets campaigns for a specific user
    /// </summary>
    public async Task<List<CampaignView>> GetCampaignsByUserAsync(int userId, bool includePublic = false)
    {
        var query = _dbContext.Campaigns
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
            GameType = (GameType)campaign.GameType,
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
        var campaign = await _dbContext.Campaigns
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.IsActive);

        if (campaign == null)
        {
            throw new BusinessException("Campaign not found.");
        }

        if (campaign.CreatedById != userId)
        {
            throw new BusinessException("You can only modify campaigns you created.");
        }

        // Check name uniqueness (excluding current campaign)
        var existingCampaign = await _dbContext.Campaigns
            .FirstOrDefaultAsync(c => c.Name == campaignRequest.Name && 
                                    c.CreatedById == userId && 
                                    c.Id != campaignId);

        if (existingCampaign != null)
        {
            throw new BusinessException($"A campaign with the name '{campaignRequest.Name}' already exists for this user.");
        }

        campaign.Name = campaignRequest.Name;
        campaign.Description = campaignRequest.Description;
        campaign.GameType = (int)campaignRequest.GameType;
        campaign.IsPublic = campaignRequest.IsPublic;
        campaign.Settings = campaignRequest.Settings;
        campaign.UpdatedDate = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GetCampaignByIdAsync(campaignId) ?? throw new BusinessException("Campaign not found after update.");
    }

    /// <summary>
    /// Deletes (deactivates) a campaign
    /// </summary>
    public async Task DeleteCampaignAsync(int campaignId, int userId)
    {
        var campaign = await _dbContext.Campaigns
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

        await _dbContext.SaveChangesAsync();
    }
}