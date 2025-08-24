namespace Cdm.Data;

using Cdm.Data.Models;
using Cdm.Data.Models.Configuration;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Existing DbSets
    public DbSet<User> Users { get; set; }

    // Campaign system DbSets
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<ContentBlock> ContentBlocks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply existing configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        // Apply campaign system configurations
        modelBuilder.ApplyConfiguration(new CampaignConfiguration());
        modelBuilder.ApplyConfiguration(new ChapterConfiguration());
        modelBuilder.ApplyConfiguration(new ContentBlockConfiguration());
    }
}