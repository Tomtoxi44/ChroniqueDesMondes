using Cdm.Business.Common.Business.Campaigns;
using Cdm.Data;
using Cdm.Data.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

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

    [Fact]
    public async Task CreateCampaignAsync_ShouldCreateCampaign()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var campaignBusiness = new CampaignBusiness(context);

        // Test implementation would go here
        Assert.True(true); // Placeholder
    }
}