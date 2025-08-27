using Cdm.Business.Common.Business.Campaigns;
using Cdm.Data;
using Xunit;
using System.Threading.Tasks;
using System;

namespace Cdm.Tests.Campaign;

public class CampaignTuDelete : CampaignTestBase
{
    [Fact]
    public async Task DeleteCampaignAsync_ShouldDeactivate_WhenUserIsOwner()
    {
        using var context = GetInMemoryDbContext();
        var user = AddUser(context);
        var campaign = AddCampaign(context, user.Id);
        var campaignBusiness = new CampaignBusiness(context);
        await campaignBusiness.DeleteCampaignAsync(campaign.Id, user.Id);
        var updated = context.Campaigns.Find(campaign.Id);
        Assert.NotNull(updated);
        Assert.False(updated!.IsActive);
    }

    [Fact]
    public async Task DeleteCampaignAsync_ShouldThrow_WhenNotOwner()
    {
        using var context = GetInMemoryDbContext();
        var user = AddUser(context);
        var otherUser = AddUser(context, "Other", "other@example.com");
        var campaign = AddCampaign(context, user.Id);
        var campaignBusiness = new CampaignBusiness(context);
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await campaignBusiness.DeleteCampaignAsync(campaign.Id, otherUser.Id);
        });
    }
}
