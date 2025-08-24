namespace Cdm.Migrations;

using Chronique.Des.Mondes.Data.Models;
using Chronique.Des.Mondes.Data.Models.Configuration;
using Data.Dnd.Models;
using Microsoft.EntityFrameworkCore;

public class MigrationContext : DbContext
{
    public MigrationContext(DbContextOptions options) : base(options)
    {
    }

    protected MigrationContext()
    {
    }

    public DbSet<CharacterDnd> CharacterDnd { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Campaign> Campaigns { get; set; }

    public DbSet<Chapter> Chapters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CampaignConfiguration());
        modelBuilder.ApplyConfiguration(new ChapterConfiguration());
    }
}
