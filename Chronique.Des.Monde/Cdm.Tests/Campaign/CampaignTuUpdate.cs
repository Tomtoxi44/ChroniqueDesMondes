using Cdm.Business.Common.Business.Campaigns;
using Cdm.Business.Common.Models.Campaign.Campaign;
using Cdm.Data;
using Xunit;
using System.Threading.Tasks;
using System;

namespace Cdm.Tests.Campaign;

public class CampaignTuUpdate : CampaignTestBase
{
    [Fact]
    public async Task UpdateCampaignAsync_ShouldUpdate_WhenUserIsOwner()
    {
        using var context = GetInMemoryDbContext();
        var user = AddUser(context);
        var campaign = AddCampaign(context, user.Id);
        var campaignBusiness = new CampaignBusiness(context);
        var updateRequest = new CampaignRequest
        {
            Name = "UpdatedName",
            Description = "UpdatedDesc",
            GameType = ValidGameType,
            IsPublic = false,
            Settings = "{}"
        };
        var result = await campaignBusiness.UpdateCampaignAsync(campaign.Id, updateRequest, user.Id);
        Assert.NotNull(result);
        Assert.Equal("UpdatedName", result.Name);
        Assert.Equal("UpdatedDesc", result.Description);
    }

    [Fact]
    public async Task UpdateCampaignAsync_ShouldThrow_WhenNotOwner()
    {
        using var context = GetInMemoryDbContext();
        var user = AddUser(context);
        var otherUser = AddUser(context, "Other", "other@example.com");
        var campaign = AddCampaign(context, user.Id);
        var campaignBusiness = new CampaignBusiness(context);
        var updateRequest = new CampaignRequest
        {
            Name = "UpdatedName",
            Description = "UpdatedDesc",
            GameType = ValidGameType,
            IsPublic = false,
            Settings = "{}"
        };
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await campaignBusiness.UpdateCampaignAsync(campaign.Id, updateRequest, otherUser.Id);
        });
    }
}
