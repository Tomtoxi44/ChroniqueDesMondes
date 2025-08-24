using Cdm.Data;
using Cdm.Data.Dnd.Models;
using Cdm.Data.Models;
using Cdm.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cdm.Data.Dnd;

public class DndDbContext : DbContext
{
    public DndDbContext(DbContextOptions<DndDbContext> options) : base(options)
    {
    }

    // Inclure tous les DbSets du AppDbContext
    public DbSet<User> Users { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<ContentBlock> ContentBlocks { get; set; }
    
    // D&D specific DbSets
    public DbSet<CharacterDnd> CharactersDnd { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply common configurations
        modelBuilder.ApplyConfiguration(new Cdm.Data.Models.Configuration.UserConfiguration());
        modelBuilder.ApplyConfiguration(new Cdm.Data.Models.Configuration.CampaignConfiguration());
        modelBuilder.ApplyConfiguration(new Cdm.Data.Models.Configuration.ChapterConfiguration());
        modelBuilder.ApplyConfiguration(new Cdm.Data.Models.Configuration.ContentBlockConfiguration());

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