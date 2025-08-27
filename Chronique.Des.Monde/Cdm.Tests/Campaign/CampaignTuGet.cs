using Cdm.Business.Common.Business.Campaigns;
using Cdm.Data;
using Xunit;
using System.Threading.Tasks;
using System;

namespace Cdm.Tests.Campaign;

public class CampaignTuGet : CampaignTestBase
{
    [Fact]
    public async Task GetCampaignByIdAsync_ShouldReturnCampaign_WhenExists()
    {
        using var context = GetInMemoryDbContext();
        var user = AddUser(context);
        var campaign = AddCampaign(context, user.Id);
        var campaignBusiness = new CampaignBusiness(context);

        var result = await campaignBusiness.GetCampaignByIdAsync(campaign.Id);
        Assert.NotNull(result);
        Assert.Equal(ValidCampaignName, result!.Name);
    }

    [Fact]
    public async Task GetCampaignByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        using var context = GetInMemoryDbContext();
        var campaignBusiness = new CampaignBusiness(context);
        var result = await campaignBusiness.GetCampaignByIdAsync(9999);
        Assert.Null(result);
    }
}
