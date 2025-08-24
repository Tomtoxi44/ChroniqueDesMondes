using Cdm.Business.Common.Business.Campaigns;
using Cdm.Business.Common.Models.Campaign;
using Cdm.Common.Enums;
using Chronique.Des.Mondes.Data;
using Chronique.Des.Mondes.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Cdm.Tests.Business;

[TestClass]
public class CampaignBusinessTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [TestMethod]
    public async Task CreateCampaignAsync_ShouldCreateCampaign_WhenValidRequest()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        
        // Create a test user
        var user = new User { UserName = "TestUser", UserEmail = "test@test.com", Password = "hashedpass" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var campaignBusiness = new CampaignBusiness(context);
        var request = new CampaignRequest
        {
            Name = "Test Campaign",
            Description = "A test campaign",
            GameType = GameType.DnD,
            IsPublic = true
        };

        // Act
        var result = await campaignBusiness.CreateCampaignAsync(request, user.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Test Campaign", result.Name);
        Assert.AreEqual("A test campaign", result.Description);
        Assert.AreEqual(GameType.DnD, result.GameType);
        Assert.IsTrue(result.IsPublic);
        Assert.AreEqual(user.Id, result.CreatedById);
        Assert.AreEqual("TestUser", result.CreatedByName);
    }

    [TestMethod]
    public async Task CreateCampaignAsync_ShouldThrowException_WhenDuplicateName()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        
        // Create a test user
        var user = new User { UserName = "TestUser", UserEmail = "test@test.com", Password = "hashedpass" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var campaignBusiness = new CampaignBusiness(context);
        var request = new CampaignRequest
        {
            Name = "Duplicate Campaign",
            GameType = GameType.Generic
        };

        // Create first campaign
        await campaignBusiness.CreateCampaignAsync(request, user.Id);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<Cdm.Common.BusinessException>(
            () => campaignBusiness.CreateCampaignAsync(request, user.Id));
    }

    [TestMethod]
    public async Task GetCampaignsByUserAsync_ShouldReturnUserCampaigns()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        
        // Create test users
        var user1 = new User { UserName = "User1", UserEmail = "user1@test.com", Password = "hashedpass" };
        var user2 = new User { UserName = "User2", UserEmail = "user2@test.com", Password = "hashedpass" };
        context.Users.AddRange(user1, user2);
        await context.SaveChangesAsync();

        var campaignBusiness = new CampaignBusiness(context);

        // Create campaigns for different users
        await campaignBusiness.CreateCampaignAsync(new CampaignRequest 
        { 
            Name = "User1 Campaign", 
            GameType = GameType.DnD 
        }, user1.Id);
        
        await campaignBusiness.CreateCampaignAsync(new CampaignRequest 
        { 
            Name = "User2 Campaign", 
            GameType = GameType.Skyrim 
        }, user2.Id);

        // Act
        var user1Campaigns = await campaignBusiness.GetCampaignsByUserAsync(user1.Id);
        var user2Campaigns = await campaignBusiness.GetCampaignsByUserAsync(user2.Id);

        // Assert
        Assert.AreEqual(1, user1Campaigns.Count);
        Assert.AreEqual("User1 Campaign", user1Campaigns[0].Name);
        Assert.AreEqual(GameType.DnD, user1Campaigns[0].GameType);

        Assert.AreEqual(1, user2Campaigns.Count);
        Assert.AreEqual("User2 Campaign", user2Campaigns[0].Name);
        Assert.AreEqual(GameType.Skyrim, user2Campaigns[0].GameType);
    }
}