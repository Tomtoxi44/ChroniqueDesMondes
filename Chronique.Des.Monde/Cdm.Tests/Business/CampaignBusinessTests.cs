using Cdm.Business.Common.Business.Campaigns;
using Cdm.Business.Common.Models.Campaign.Campaign;
using Cdm.Data;
using Xunit;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace Cdm.Tests.Business;

public class CampaignBusinessTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private Cdm.Data.Models.User AddUser(AppDbContext context, string name = "TestUser", string email = "test@example.com")
    {
        var user = new Cdm.Data.Models.User { UserName = name, UserEmail = email, Password = "hashed" };
        context.Users.Add(user);
        context.SaveChanges();
        return user;
    }

    [Fact]
    public async Task CreateCampaignAsync_ShouldCreateCampaign()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var user = AddUser(context);
        var campaignBusiness = new CampaignBusiness(context);
        var request = new CampaignRequest
        {
            Name = "Campagne Test",
            Description = "Description test",
            GameType = Cdm.Common.Enums.GameType.Generic,
            IsPublic = true,
            Settings = "{}"
        };

        // Act
        var result = await campaignBusiness.CreateCampaignAsync(request, user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(user.Id, result.CreatedById);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetCampaignByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        using var context = GetInMemoryDbContext();
        var campaignBusiness = new CampaignBusiness(context);

        var result = await campaignBusiness.GetCampaignByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCampaignByIdAsync_ShouldReturnCampaign_WhenExists()
    {
        using var context = GetInMemoryDbContext();
        var user = AddUser(context);
        var campaign = new Cdm.Data.Models.Campaign
        {
            Name = "Test",
            Description = "Desc",
            GameType = Cdm.Common.Enums.GameType.Generic,
            IsPublic = false,
            CreatedById = user.Id,
            Settings = null,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        context.Campaigns.Add(campaign);
        context.SaveChanges();
        var campaignBusiness = new CampaignBusiness(context);

        var result = await campaignBusiness.GetCampaignByIdAsync(campaign.Id);
        Assert.NotNull(result);
        Assert.Equal(campaign.Name, result!.Name);
    }
}