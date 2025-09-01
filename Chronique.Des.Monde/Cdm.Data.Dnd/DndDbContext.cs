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

    // Character DbSets
    public DbSet<CharacterDnd> CharactersDnd { get; set; }

    // Spell DbSets
    public DbSet<SpellDnd> SpellsDnd { get; set; }

    // Equipment DbSets
    public DbSet<EquipmentDnd> EquipmentDnd { get; set; }

    // Import shared entities from AppDbContext for navigation
    public DbSet<Cdm.Data.Models.User> Users { get; set; }
    public DbSet<Cdm.Data.Models.Campaign> Campaigns { get; set; }
    public DbSet<Cdm.Data.Models.Chapter> Chapters { get; set; }
    public DbSet<Cdm.Data.Models.ContentBlock> ContentBlocks { get; set; }

    // Import character liaison entities
    public DbSet<Cdm.Data.Common.Models.CharacterSpells> CharacterSpells { get; set; }
    public DbSet<Cdm.Data.Common.Models.CharacterInventory> CharacterInventory { get; set; }

    // Import equipment exchange entities
    public DbSet<Cdm.Data.Common.Models.EquipmentOffer> EquipmentOffers { get; set; }
    public DbSet<Cdm.Data.Common.Models.EquipmentTrade> EquipmentTrades { get; set; }

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
        
        // Apply character liaison configurations
        modelBuilder.ApplyConfiguration(new Cdm.Data.Common.Models.Configuration.CharacterSpellsConfiguration());
        modelBuilder.ApplyConfiguration(new Cdm.Data.Common.Models.Configuration.CharacterInventoryConfiguration());

        // Apply equipment exchange configurations
        modelBuilder.ApplyConfiguration(new Cdm.Data.Common.Models.Configuration.EquipmentOfferConfiguration());
        modelBuilder.ApplyConfiguration(new Cdm.Data.Common.Models.Configuration.EquipmentTradeConfiguration());
    }
}