using Cdm.Business.Common.Business.Campaigns;
using Cdm.Business.Common.Models.Campaign.Campaign;
using Cdm.Data;
using Xunit;
using System.Threading.Tasks;
using System;

namespace Cdm.Tests.Campaign;

public class CampaignTuCreation : CampaignTestBase
{
    [Fact]
    public async Task CreateCampaignAsync_ShouldCreateCampaign()
    {
        using var context = GetInMemoryDbContext();
        var user = AddUser(context);
        var campaignBusiness = new CampaignBusiness(context);
        var request = new CampaignRequest
        {
            Name = ValidCampaignName,
            Description = ValidCampaignDescription,
            GameType = ValidGameType,
            IsPublic = true,
            Settings = "{}"
        };

        var result = await campaignBusiness.CreateCampaignAsync(request, user.Id);

        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(user.Id, result.CreatedById);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task CreateCampaignAsync_ShouldThrow_WhenUserNotExist()
    {
        using var context = GetInMemoryDbContext();
        var campaignBusiness = new CampaignBusiness(context);
        var request = new CampaignRequest
        {
            Name = ValidCampaignName,
            Description = ValidCampaignDescription,
            GameType = ValidGameType,
            IsPublic = true,
            Settings = "{}"
        };
        // userId inexistant
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await campaignBusiness.CreateCampaignAsync(request, 9999);
        });
    }
}
