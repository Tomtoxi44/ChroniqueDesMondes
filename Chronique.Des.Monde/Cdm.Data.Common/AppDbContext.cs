namespace Cdm.Data;

using Cdm.Data.Models;
using Cdm.Data.Models.Configuration;
using Cdm.Data.Common.Models;
using Cdm.Data.Common.Models.Configuration;
using Cdm.Data.Common.Models.Combat;
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

    // Invitation system DbSets
    public DbSet<CampaignInvitation> CampaignInvitations { get; set; }
    public DbSet<CampaignParticipant> CampaignParticipants { get; set; }

    // Spell system DbSets (classe de base)
    public DbSet<ASpell> Spells { get; set; }

    // Equipment system DbSets (classe de base)
    public DbSet<AEquipment> Equipment { get; set; }
    public DbSet<EquipmentOffer> EquipmentOffers { get; set; }
    public DbSet<EquipmentTrade> EquipmentTrades { get; set; }
    public DbSet<CharacterInventory> CharacterInventory { get; set; }

    // Character ↔ Spell liaison DbSets
    public DbSet<CharacterSpells> CharacterSpells { get; set; }

    // Combat system DbSets ⚔️
    public DbSet<CombatSession> CombatSessions { get; set; }
    public DbSet<CombatParticipant> CombatParticipants { get; set; }
    public DbSet<CombatAction> CombatActions { get; set; }
    public DbSet<CombatStatusEffect> CombatStatusEffects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply existing configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        // Apply campaign system configurations
        modelBuilder.ApplyConfiguration(new CampaignConfiguration());
        modelBuilder.ApplyConfiguration(new ChapterConfiguration());
        modelBuilder.ApplyConfiguration(new ContentBlockConfiguration());

        // Apply invitation system configurations
        modelBuilder.ApplyConfiguration(new CampaignInvitationConfiguration());
        modelBuilder.ApplyConfiguration(new CampaignParticipantConfiguration());

        // Apply spell system configurations (classe de base)
        modelBuilder.ApplyConfiguration(new ASpellConfiguration());

        // Apply equipment system configurations (classe de base)
        modelBuilder.ApplyConfiguration(new AEquipmentConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentOfferConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentTradeConfiguration());
        modelBuilder.ApplyConfiguration(new CharacterInventoryConfiguration());

        // Apply character liaison configurations
        modelBuilder.ApplyConfiguration(new CharacterSpellsConfiguration());

        // Combat system configurations ⚔️
        ConfigureCombatEntities(modelBuilder);
    }

    /// <summary>
    /// Configure les entités de combat avec les relations et contraintes appropriées
    /// </summary>
    private static void ConfigureCombatEntities(ModelBuilder modelBuilder)
    {
        // Configuration CombatSession
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

        // Configuration CombatParticipant
        modelBuilder.Entity<CombatParticipant>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ParticipantType)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Relations
            entity.HasOne(e => e.Combat)
                .WithMany(e => e.Participants)
                .HasForeignKey(e => e.CombatId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index pour les requêtes fréquentes
            entity.HasIndex(e => e.CombatId);
            entity.HasIndex(e => new { e.CombatId, e.ParticipantType });
            entity.HasIndex(e => new { e.CombatId, e.Initiative });
        });

        // Configuration CombatAction
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
                .OnDelete(DeleteBehavior.Restrict); // Éviter la suppression en cascade

            // Index pour les requêtes fréquentes
            entity.HasIndex(e => e.CombatId);
            entity.HasIndex(e => e.ActorId);
            entity.HasIndex(e => new { e.CombatId, e.Round });
            entity.HasIndex(e => e.ExecutedAt);
        });

        // Configuration CombatStatusEffect
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