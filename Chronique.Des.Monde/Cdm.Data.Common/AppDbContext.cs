namespace Cdm.Data;

using Cdm.Data.Models;
using Cdm.Data.Models.Configuration;
using Cdm.Data.Dnd.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Existing DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<CharacterDnd> CharactersDnd { get; set; }

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

        // Configure inheritance for characters
        modelBuilder.Entity<ACharacter>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<CharacterDnd>("CharacterDnd");

        // Configure Character specific properties for NPCs
        modelBuilder.Entity<ACharacter>()
            .HasIndex(c => c.ChapterId)
            .HasDatabaseName("IX_Characters_ChapterId");

        modelBuilder.Entity<ACharacter>()
            .HasIndex(c => c.IsNpc)
            .HasDatabaseName("IX_Characters_IsNpc");

        modelBuilder.Entity<ACharacter>()
            .HasIndex(c => c.IsHostile)
            .HasDatabaseName("IX_Characters_IsHostile");

        modelBuilder.Entity<ACharacter>()
            .HasIndex(c => c.GameType)
            .HasDatabaseName("IX_Characters_GameType");

        modelBuilder.Entity<ACharacter>()
            .HasIndex(c => c.IsSystemCharacter)
            .HasDatabaseName("IX_Characters_IsSystemCharacter");

        // Configure GameType as enum
        modelBuilder.Entity<ACharacter>()
            .Property(c => c.GameType)
            .HasConversion<int>();

        // Configure defaults
        modelBuilder.Entity<ACharacter>()
            .Property(c => c.IsNpc)
            .HasDefaultValue(false);

        modelBuilder.Entity<ACharacter>()
            .Property(c => c.IsHostile)
            .HasDefaultValue(false);

        modelBuilder.Entity<ACharacter>()
            .Property(c => c.IsSystemCharacter)
            .HasDefaultValue(false);

        modelBuilder.Entity<ACharacter>()
            .Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}