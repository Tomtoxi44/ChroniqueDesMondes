using Cdm.Data;
using Cdm.Data.Dnd.Models;
using Cdm.Data.Models;
using Cdm.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Cdm.Data.Dnd.Models.Configuration;

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
    public DbSet<SpellDnd> SpellsDnd { get; set; }
    public DbSet<EquipmentDnd> EquipmentDnd { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply D&D specific configurations
        // modelBuilder.ApplyConfiguration(new CharacterDndConfiguration());
        modelBuilder.ApplyConfiguration(new SpellDndConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentDndConfiguration());

        // Apply base configurations
        modelBuilder.ApplyConfiguration(new Cdm.Data.Common.Models.Configuration.ASpellConfiguration());
        modelBuilder.ApplyConfiguration(new Cdm.Data.Common.Models.Configuration.AEquipmentConfiguration());
    }
}