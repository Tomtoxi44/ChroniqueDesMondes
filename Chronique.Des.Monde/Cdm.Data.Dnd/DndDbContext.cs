using Cdm.Data;
using Cdm.Data.Dnd.Models;
using Cdm.Data.Models;
using Cdm.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Cdm.Data.Dnd.Models.Configuration;
using Cdm.Data.Common.Models.Combat;

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

    // Combat System DbSets ⚔️
    public DbSet<CombatSession> CombatSessions { get; set; }
    public DbSet<CombatParticipant> CombatParticipants { get; set; }
    public DbSet<CombatAction> CombatActions { get; set; }
    public DbSet<CombatStatusEffect> CombatStatusEffects { get; set; }

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

        // Combat system configurations with D&D specific relations ⚔️
        ConfigureDndCombatEntities(modelBuilder);
    }

    /// <summary>
    /// Configure les entités de combat avec les relations spécifiques D&D
    /// </summary>
    private static void ConfigureDndCombatEntities(ModelBuilder modelBuilder)
    {
        // Configuration CombatSession (héritée du AppDbContext)
        modelBuilder.Entity<CombatSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.SessionId)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.GameType)
                .IsRequired()
                .HasConversion<string>();

            // Index pour les requêtes fréquentes
            entity.HasIndex(e => e.SessionId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.SessionId, e.Status });
        });

        // Configuration CombatParticipant avec relation vers CharacterDnd
        modelBuilder.Entity<CombatParticipant>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ParticipantType)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Relations combat
            entity.HasOne(e => e.Combat)
                .WithMany(e => e.Participants)
                .HasForeignKey(e => e.CombatId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation spécifique D&D vers CharacterDnd
            entity.HasOne<CharacterDnd>()
                .WithMany()
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index pour les requêtes fréquentes
            entity.HasIndex(e => e.CombatId);
            entity.HasIndex(e => new { e.CombatId, e.ParticipantType });
            entity.HasIndex(e => new { e.CombatId, e.Initiative });
            entity.HasIndex(e => e.CharacterId);
        });

        // Configuration CombatAction (héritée du AppDbContext)
        modelBuilder.Entity<CombatAction>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ActionType)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.ActionName)
                .IsRequired()
                .HasMaxLength(100);

            // Relations
            entity.HasOne(e => e.Combat)
                .WithMany(e => e.Actions)
                .HasForeignKey(e => e.CombatId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Actor)
                .WithMany(e => e.Actions)
                .HasForeignKey(e => e.ActorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Target)
                .WithMany()
                .HasForeignKey(e => e.TargetId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relations spécifiques D&D vers équipements et sorts
            entity.HasOne<EquipmentDnd>()
                .WithMany()
                .HasForeignKey(e => e.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<SpellDnd>()
                .WithMany()
                .HasForeignKey(e => e.SpellId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index pour les requêtes fréquentes
            entity.HasIndex(e => e.CombatId);
            entity.HasIndex(e => e.ActorId);
            entity.HasIndex(e => new { e.CombatId, e.Round });
            entity.HasIndex(e => e.ExecutedAt);
            entity.HasIndex(e => e.EquipmentId);
            entity.HasIndex(e => e.SpellId);
        });

        // Configuration CombatStatusEffect (héritée du AppDbContext)
        modelBuilder.Entity<CombatStatusEffect>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.EffectType)
                .IsRequired()
                .HasMaxLength(20);

            // Relations
            entity.HasOne(e => e.Participant)
                .WithMany()
                .HasForeignKey(e => e.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SourceParticipant)
                .WithMany()
                .HasForeignKey(e => e.SourceParticipantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index pour les requêtes fréquentes
            entity.HasIndex(e => e.ParticipantId);
            entity.HasIndex(e => new { e.ParticipantId, e.IsActive });
            entity.HasIndex(e => e.EffectType);
        });
    }
}