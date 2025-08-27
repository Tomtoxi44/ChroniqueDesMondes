using Cdm.Data;
using Cdm.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Cdm.Business.Common.Models.Campaign.Campaign;
using Cdm.Common.Enums;

namespace Cdm.Tests.Campaign;

public abstract class CampaignTestBase
{
    protected AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    public static readonly string ValidCampaignName = "Campagne Test";
    public static readonly string ValidCampaignDescription = "Description test";
    public static readonly GameType ValidGameType = GameType.Generic;

    protected Cdm.Data.Models.User AddUser(AppDbContext context, string name = "TestUser", string email = "test@example.com")
    {
        var user = new Cdm.Data.Models.User { UserName = name, UserEmail = email, Password = "hashed" };
        context.Users.Add(user);
        context.SaveChanges();
        return user;
    }

    protected Cdm.Data.Models.Campaign AddCampaign(AppDbContext context, int createdById)
    {
        var campaign = new Cdm.Data.Models.Campaign
        {
            Name = ValidCampaignName,
            Description = ValidCampaignDescription,
            GameType = ValidGameType,
            IsPublic = true,
            CreatedById = createdById,
            Settings = "{}",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        context.Campaigns.Add(campaign);
        context.SaveChanges();
        return campaign;
    }
}
