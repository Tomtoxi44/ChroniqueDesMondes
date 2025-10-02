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

    // 🔧 TABLES PRINCIPALES D&D - MINIMUM POUR TESTER LE SEEDER
    public DbSet<CharacterDnd> CharactersDnd { get; set; }
    public DbSet<SpellDnd> SpellsDnd { get; set; }
    public DbSet<EquipmentDnd> EquipmentDnd { get; set; }

    // 🔧 Tables partagées minimales
    public DbSet<Cdm.Data.Models.User> Users { get; set; }
    public DbSet<Cdm.Data.Models.Campaign> Campaigns { get; set; }
    public DbSet<Cdm.Data.Models.Chapter> Chapters { get; set; }
    public DbSet<Cdm.Data.Models.ContentBlock> ContentBlocks { get; set; }

    // 🔧 Tables de liaison (pour éviter les erreurs des services Common)
    public DbSet<Cdm.Data.Common.Models.CharacterSpells> CharacterSpells { get; set; }
    public DbSet<Cdm.Data.Common.Models.CharacterInventory> CharacterInventory { get; set; }
    public DbSet<Cdm.Data.Common.Models.EquipmentOffer> EquipmentOffers { get; set; }
    public DbSet<Cdm.Data.Common.Models.EquipmentTrade> EquipmentTrades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration pour les tables D&D spécialisées SEULEMENT
        modelBuilder.ApplyConfiguration(new SpellDndConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentDndConfiguration());

        // Configuration minimale pour éviter les recréations
        ConfigureExistingTables(modelBuilder);
    }

    /// <summary>
    /// Configure les tables existantes avec configuration minimale
    /// </summary>
    private static void ConfigureExistingTables(ModelBuilder modelBuilder)
    {
        // 🔧 Configuration TPH : Les types dérivés partagent les tables des parents
        // CharacterDnd hérite de ACharacter → même table "ACharacter"
        modelBuilder.Entity<CharacterDnd>(entity =>
        {
            entity.ToTable("ACharacter");  // ✅ Même table que le parent
        });
        
        // SpellDnd hérite de ASpell → même table "ASpell"  
        modelBuilder.Entity<SpellDnd>(entity =>
        {
            entity.ToTable("ASpell");  // ✅ Même table que le parent
        });
        
        // EquipmentDnd hérite de AEquipment → même table "AEquipment"
        modelBuilder.Entity<EquipmentDnd>(entity =>
        {
            entity.ToTable("AEquipment");  // ✅ Même table que le parent
        });

        modelBuilder.Entity<Cdm.Data.Models.User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Cdm.Data.Models.Campaign>(entity =>
        {
            entity.ToTable("Campaigns");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Cdm.Data.Models.Chapter>(entity =>
        {
            entity.ToTable("Chapters");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Cdm.Data.Models.ContentBlock>(entity =>
        {
            entity.ToTable("ContentBlocks");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Cdm.Data.Common.Models.CharacterSpells>(entity =>
        {
            entity.ToTable("CharacterSpells");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Cdm.Data.Common.Models.CharacterInventory>(entity =>
        {
            entity.ToTable("CharacterInventory");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Cdm.Data.Common.Models.EquipmentOffer>(entity =>
        {
            entity.ToTable("EquipmentOffers");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Cdm.Data.Common.Models.EquipmentTrade>(entity =>
        {
            entity.ToTable("EquipmentTrades");
            entity.HasKey(e => e.Id);
        });
    }
}