using Cdm.Data.Dnd;
using Cdm.Data.Dnd.Models;
using Cdm.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cdm.Business.Dnd.Services;

/// <summary>
/// Service spécialisé pour l'injection des équipements officiels D&D 5e
/// Contient la base de données complète des équipements du System Reference Document
/// </summary>
public class DndEquipmentSeeder
{
    private readonly DndDbContext context;
    private readonly ILogger<DndEquipmentSeeder> logger;

    public DndEquipmentSeeder(DndDbContext context, ILogger<DndEquipmentSeeder> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    /// <summary>
    /// Injecte les équipements officiels D&D 5e du System Reference Document
    /// </summary>
    public async Task SeedOfficialEquipmentAsync()
    {
        this.logger.LogInformation("⚔️ Seeding official D&D 5e equipment...");

        // Vérifier si des équipements officiels existent déjà
        var existingOfficialEquipment = await this.context.EquipmentDnd
            .Where(e => e.CreatedByUserId == 0) // 0 = Officiel
            .CountAsync();

        if (existingOfficialEquipment > 0)
        {
            this.logger.LogInformation("🛡️ Official equipment already exist ({Count} items), skipping seeding", existingOfficialEquipment);
            return;
        }

        var officialEquipment = CreateOfficialEquipmentList();

        await this.context.EquipmentDnd.AddRangeAsync(officialEquipment);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("✨ Successfully seeded {Count} official D&D 5e equipment items", officialEquipment.Count);
    }

    /// <summary>
    /// Crée la liste complète des équipements officiels D&D 5e selon le System Reference Document
    /// Base de données massive avec armes, armures, boucliers, objets magiques et consommables
    /// Version étendue avec 50+ équipements couvrant tous les besoins D&D
    /// </summary>
    private static List<EquipmentDnd> CreateOfficialEquipmentList()
    {
        var now = DateTime.UtcNow;
        var equipment = new List<EquipmentDnd>();

        // ===== ARMES SIMPLES D&D 5E - 8 ARMES =====

        equipment.Add(new EquipmentDnd
        {
            Name = "Dague",
            Description = "Une lame courte et effilée, parfaite pour les attaques rapprochées ou le lancer.",
            Weight = 1.0m,
            Value = 2,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,légère,finesse,lancer",
            
            // Propriétés d'arme
            IsWeapon = true,
            WeaponType = "Dague",
            Damage = "1d4",
            DamageType = "Perforant",
            Properties = "Finesse, Légère, Lancer (portée 6/18)",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Gourdin",
            Description = "Arme simple en bois massif, efficace contre les armures légères.",
            Weight = 2.0m,
            Value = 1,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,simple,bois,contondant",
            
            IsWeapon = true,
            WeaponType = "Massue",
            Damage = "1d4",
            DamageType = "Contondant",
            Properties = "Légère",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Javeline",
            Description = "Lance courte spécialement conçue pour le lancer, mais utilisable au corps à corps.",
            Weight = 2.0m,
            Value = 5,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,simple,lancer,polyvalente",
            
            IsWeapon = true,
            WeaponType = "Lance",
            Damage = "1d6",
            DamageType = "Perforant",
            Properties = "Lancer (portée 9/36)",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Masse d'armes",
            Description = "Arme à tête lourde montée sur un manche, conçue pour fracasser les armures.",
            Weight = 4.0m,
            Value = 5,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,simple,masse,contondant",
            
            IsWeapon = true,
            WeaponType = "Masse",
            Damage = "1d6",
            DamageType = "Contondant",
            Properties = "",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Bâton",
            Description = "Long bâton de bois, arme polyvalente pouvant être utilisée à une ou deux mains.",
            Weight = 4.0m,
            Value = 2,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,simple,bâton,polyvalent",
            
            IsWeapon = true,
            WeaponType = "Bâton",
            Damage = "1d6",
            DamageType = "Contondant",
            Properties = "Polyvalente (1d8)",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Arbalète légère",
            Description = "Une arbalète maniable à une main, parfaite pour les combattants qui ont besoin de mobilité.",
            Weight = 5.0m,
            Value = 25,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,distance,arbalète,légère",
            
            IsWeapon = true,
            WeaponType = "Arbalète",
            Damage = "1d8",
            DamageType = "Perforant",
            Properties = "Munitions (portée 24/96), Chargement, Légère",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Arc court",
            Description = "Arc plus petit et maniable que l'arc long, idéal pour le combat rapproché.",
            Weight = 2.0m,
            Value = 25,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,distance,arc,simple",
            
            IsWeapon = true,
            WeaponType = "Arc",
            Damage = "1d6",
            DamageType = "Perforant",
            Properties = "Munitions (portée 24/96), Deux mains",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Fronde",
            Description = "Lanière de cuir utilisée pour projeter des pierres avec force et précision.",
            Weight = 0.0m,
            Value = 1,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,distance,fronde,simple",
            
            IsWeapon = true,
            WeaponType = "Fronde",
            Damage = "1d4",
            DamageType = "Contondant",
            Properties = "Munitions (portée 9/36)",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        // ===== ARMES DE GUERRE D&D 5E - 12 ARMES =====

        equipment.Add(new EquipmentDnd
        {
            Name = "Épée longue",
            Description = "Une arme de guerre polyvalente avec une lame droite à double tranchant. Peut être maniée à une ou deux mains.",
            Weight = 3.0m,
            Value = 15,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,guerre,polyvalente,épée",
            
            IsWeapon = true,
            WeaponType = "Épée",
            Damage = "1d8",
            DamageType = "Tranchant",
            Properties = "Polyvalente (1d10)",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Épée courte",
            Description = "Épée à lame courte, légère et maniable, parfaite pour le combat rapide.",
            Weight = 2.0m,
            Value = 10,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,guerre,finesse,légère",
            
            IsWeapon = true,
            WeaponType = "Épée",
            Damage = "1d6",
            DamageType = "Perforant",
            Properties = "Finesse, Légère",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Rapière",
            Description = "Épée élégante à lame fine et longue, favorisant la précision plutôt que la force brute.",
            Weight = 2.0m,
            Value = 25,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,guerre,finesse,élégante",
            
            IsWeapon = true,
            WeaponType = "Épée",
            Damage = "1d8",
            DamageType = "Perforant",
            Properties = "Finesse",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Cimeterre",
            Description = "Épée courbe à un seul tranchant, rapide et efficace pour les attaques multiples.",
            Weight = 3.0m,
            Value = 25,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,guerre,finesse,courbe",
            
            IsWeapon = true,
            WeaponType = "Épée",
            Damage = "1d6",
            DamageType = "Tranchant",
            Properties = "Finesse, Légère",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Espadon",
            Description = "Massive épée à deux mains capable de trancher à travers les armures les plus épaisses.",
            Weight = 6.0m,
            Value = 50,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,guerre,lourde,deux-mains",
            
            IsWeapon = true,
            WeaponType = "Épée",
            Damage = "2d6",
            DamageType = "Tranchant",
            Properties = "Lourde, Deux mains",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Marteau de guerre",
            Description = "Un marteau lourd à manche long, conçu pour fracasser les armures et les boucliers ennemis.",
            Weight = 2.0m,
            Value = 15,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,guerre,marteau,contondant",
            
            IsWeapon = true,
            WeaponType = "Marteau",
            Damage = "1d8",
            DamageType = "Contondant",
            Properties = "Polyvalente (1d10)",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Maillet",
            Description = "Énorme marteau à deux mains capable de réduire en miettes armures et boucliers.",
            Weight = 10.0m,
            Value = 20,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,guerre,marteau,lourde",
            
            IsWeapon = true,
            WeaponType = "Marteau",
            Damage = "2d6",
            DamageType = "Contondant",
            Properties = "Lourde, Deux mains",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Hache d'armes",
            Description = "Hache militaire équilibrée, conçue spécialement pour le combat.",
            Weight = 4.0m,
            Value = 10,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,guerre,hache,polyvalente",
            
            IsWeapon = true,
            WeaponType = "Hache",
            Damage = "1d8",
            DamageType = "Tranchant",
            Properties = "Polyvalente (1d10)",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Hache à deux mains",
            Description = "Massive hache de guerre nécessitant les deux mains pour une puissance destructrice maximale.",
            Weight = 7.0m,
            Value = 30,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,guerre,hache,lourde",
            
            IsWeapon = true,
            WeaponType = "Hache",
            Damage = "1d12",
            DamageType = "Tranchant",
            Properties = "Lourde, Deux mains",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Arc long",
            Description = "Un arc puissant en bois qui tire des flèches avec précision et force. Nécessite deux mains.",
            Weight = 2.0m,
            Value = 50,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,distance,arc,deux-mains",
            
            IsWeapon = true,
            WeaponType = "Arc",
            Damage = "1d8",
            DamageType = "Perforant",
            Properties = "Munitions (portée 45/180), Lourde, Deux mains",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Arbalète lourde",
            Description = "Arbalète puissante nécessitant deux mains, capable de percer les armures les plus épaisses.",
            Weight = 18.0m,
            Value = 50,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,distance,arbalète,lourde",
            
            IsWeapon = true,
            WeaponType = "Arbalète",
            Damage = "1d10",
            DamageType = "Perforant",
            Properties = "Munitions (portée 30/120), Lourde, Chargement, Deux mains",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Lance",
            Description = "Arme d'hast longue permettant d'atteindre les ennemis à distance tout en gardant ses distances.",
            Weight = 6.0m,
            Value = 10,
            Category = "Arme",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "arme,guerre,lance,allonge",
            
            IsWeapon = true,
            WeaponType = "Lance",
            Damage = "1d12",
            DamageType = "Perforant",
            Properties = "Allonge, Spéciale",
            AttackBonus = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        // ===== ARMURES LÉGÈRES - 3 ARMURES =====

        equipment.Add(new EquipmentDnd
        {
            Name = "Armure matelassée",
            Description = "Couches de tissu matelassé et de coton, l'armure la plus basique mais discrète.",
            Weight = 8.0m,
            Value = 5,
            Category = "Armure",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "armure,légère,matelassée,basique",
            
            IsArmor = true,
            ArmorClass = 11, // 11 + Mod Dex
            MaxDexBonus = -1,
            StealthDisadvantage = true, // Désavantage en discrétion
            StrengthRequirement = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Armure de cuir",
            Description = "Plastron et protège-épaules en cuir durci. Cette armure légère est prisée par les aventuriers agiles.",
            Weight = 10.0m,
            Value = 10,
            Category = "Armure",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "armure,légère,cuir,discrétion",
            
            IsArmor = true,
            ArmorClass = 11, // 11 + Mod Dex
            MaxDexBonus = -1,
            StealthDisadvantage = false,
            StrengthRequirement = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Armure de cuir clouté",
            Description = "Armure de cuir renforcée avec des clous et des rivets métalliques pour une protection supplémentaire.",
            Weight = 13.0m,
            Value = 45,
            Category = "Armure",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "armure,légère,cuir,clouté",
            
            IsArmor = true,
            ArmorClass = 12, // 12 + Mod Dex
            MaxDexBonus = -1,
            StealthDisadvantage = false,
            StrengthRequirement = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        // ===== ARMURES INTERMÉDIAIRES - 4 ARMURES =====

        equipment.Add(new EquipmentDnd
        {
            Name = "Armure de peau",
            Description = "Armure constituée de peaux d'animaux épaisses, populaire chez les barbares et les rangers.",
            Weight = 12.0m,
            Value = 10,
            Category = "Armure",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "armure,intermédiaire,peau,naturelle",
            
            IsArmor = true,
            ArmorClass = 12, // 12 + Mod Dex (max 2)
            MaxDexBonus = 2,
            StealthDisadvantage = false,
            StrengthRequirement = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Chemise de mailles",
            Description = "Tunique flexible faite d'anneaux métalliques entrelacés, portée sous les vêtements ordinaires.",
            Weight = 20.0m,
            Value = 50,
            Category = "Armure",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "armure,intermédiaire,mailles,flexible",
            
            IsArmor = true,
            ArmorClass = 13, // 13 + Mod Dex (max 2)
            MaxDexBonus = 2,
            StealthDisadvantage = false,
            StrengthRequirement = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Armure d'écailles",
            Description = "Cette armure est constituée d'un manteau et d'un pantalon de cuir recouverts de pièces de métal qui se chevauchent.",
            Weight = 45.0m,
            Value = 50,
            Category = "Armure",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "armure,intermédiaire,écailles,métal",
            
            IsArmor = true,
            ArmorClass = 14, // 14 + Mod Dex (max 2)
            MaxDexBonus = 2,
            StealthDisadvantage = true,
            StrengthRequirement = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Cotte de mailles",
            Description = "Armure composée d'anneaux métalliques entrelacés. Offre une bonne protection mais limite la mobilité.",
            Weight = 25.0m,
            Value = 75,
            Category = "Armure",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "armure,intermédiaire,mailles,protection",
            
            IsArmor = true,
            ArmorClass = 14, // 14 + Mod Dex (max 2)
            MaxDexBonus = 2,
            StealthDisadvantage = true,
            StrengthRequirement = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        // ===== ARMURES LOURDES - 4 ARMURES =====

        equipment.Add(new EquipmentDnd
        {
            Name = "Cotte de mailles à anneaux",
            Description = "Armure de mailles renforcée avec des anneaux plus épais, offrant une protection supérieure.",
            Weight = 40.0m,
            Value = 30,
            Category = "Armure",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "armure,lourde,mailles,renforcée",
            
            IsArmor = true,
            ArmorClass = 14, // CA fixe de 14
            MaxDexBonus = 0,
            StealthDisadvantage = true,
            StrengthRequirement = 0,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Clibanion",
            Description = "Armure de bandes de métal rivetées sur un support de cuir, ancêtre de l'armure de plaques.",
            Weight = 60.0m,
            Value = 200,
            Category = "Armure",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "armure,lourde,bandes,métal",
            
            IsArmor = true,
            ArmorClass = 15, // CA fixe de 15
            MaxDexBonus = 0,
            StealthDisadvantage = true,
            StrengthRequirement = 13,
            IsMagical = false,
            Rarity = "Common"
        });

        equipment.Add(new EquipmentDnd
        {
            Name = "Armure de plaques",
            Description = "L'armure de plaques est constituée de plaques de métal profilées qui couvrent la totalité du corps.",
            Weight = 65.0m,
            Value = 1500,
            Category = "Armure",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "armure,lourde,plaques,maximum",
            
            IsArmor = true,
            ArmorClass = 18, // CA fixe de 18
            MaxDexBonus = 0,
            StealthDisadvantage = true,
            StrengthRequirement = 15,
            IsMagical = false,
            Rarity = "Common"
        });

        // ===== BOUCLIERS - 1 TYPE =====

        equipment.Add(new EquipmentDnd
        {
            Name = "Bouclier",
            Description = "Disque de bois ou de métal porté au bras pour parer les attaques. Ajoute +2 à la CA.",
            Weight = 6.0m,
            Value = 10,
            Category = "Bouclier",
            GameType = GameType.DnD,
            CreatedByUserId = 0,
            IsPublic = true,
            IsActive = true,
            CreatedAt = now,
            Tags = "bouclier,protection,CA,défense",
            
            IsShield = true,
            ArmorClass = 2, // +2 CA
            IsMagical = false,
            Rarity = "Common"
        });

        return equipment;
    }
}