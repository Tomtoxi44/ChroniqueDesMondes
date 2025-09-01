using Cdm.Data.Dnd;
using Cdm.Data.Dnd.Models;
using Cdm.Data.Models;
using Cdm.Business.Dnd.Models.Equipment;
using Cdm.Common;
using Cdm.Common.Enums;
using Cmd.Abstraction.Equipment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cdm.Business.Dnd.Business;

/// <summary>
/// Service métier spécialisé pour la gestion des équipements D&D
/// Implémente IEquipmentBusiness pour l'injection par clé, suit le pattern SpellDndBusiness
/// </summary>
public class EquipmentDndBusiness : IEquipmentBusiness
{
    private readonly DndDbContext context;
    private readonly ILogger<EquipmentDndBusiness> logger;

    public EquipmentDndBusiness(DndDbContext context, ILogger<EquipmentDndBusiness> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<IEnumerable<IEquipmentView>> GetAllEquipmentsByUserId(int userId, GameType gameType)
    {
        this.logger.LogInformation("Getting all D&D equipment for user {UserId}", userId);

        var equipment = await this.context.EquipmentDnd
            .Where(e => e.IsActive &&
                       (e.IsPublic || e.CreatedByUserId == userId))
            .OrderBy(e => e.Category)
            .ThenBy(e => e.Name)
            .ToListAsync();

        return equipment.Select(this.MapToView).ToList();
    }

    public async Task<IEquipmentView> GetEquipmentById(int equipmentId, int userId)
    {
        this.logger.LogInformation("Getting D&D equipment {EquipmentId} for user {UserId}", equipmentId, userId);

        var equipment = await this.context.EquipmentDnd
            .Include(e => e.CreatedBy)
            .FirstOrDefaultAsync(e => e.Id == equipmentId && e.IsActive);

        if (equipment == null)
            throw new InvalidOperationException($"Equipment with ID {equipmentId} not found");

        // Vérifier les droits d'accès
        if (!equipment.IsPublic && equipment.CreatedByUserId != userId)
            throw new BusinessException("You don't have access to this equipment");

        return this.MapToView(equipment);
    }

    public async Task<IEquipmentView> CreateEquipment(EquipmentRequest equipmentRequest, int userId)
    {
        this.logger.LogInformation("Creating D&D equipment {Name} for user {UserId}", equipmentRequest.Name, userId);

        // Validation métier D&D
        if (string.IsNullOrWhiteSpace(equipmentRequest.Name))
            throw new BusinessException("Le nom de l'équipement est obligatoire.");

        if (string.IsNullOrWhiteSpace(equipmentRequest.Description))
            throw new BusinessException("La description de l'équipement est obligatoire.");

        if (string.IsNullOrWhiteSpace(equipmentRequest.Category))
            throw new BusinessException("La catégorie de l'équipement est obligatoire.");

        // Vérifier l'unicité du nom pour cet utilisateur
        var existingEquipment = await this.context.EquipmentDnd
            .FirstOrDefaultAsync(e => e.Name == equipmentRequest.Name && 
                                    e.CreatedByUserId == userId && 
                                    e.IsActive);

        if (existingEquipment != null)
            throw new BusinessException($"Vous avez déjà un équipement D&D nommé '{equipmentRequest.Name}'.");

        // Créer l'équipement D&D
        var equipment = new EquipmentDnd
        {
            Name = equipmentRequest.Name,
            Description = equipmentRequest.Description,
            ImageUrl = equipmentRequest.ImageUrl,
            GameType = GameType.DnD,
            CreatedByUserId = userId,
            IsPublic = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Category = equipmentRequest.Category,
            Weight = equipmentRequest.Weight,
            Value = equipmentRequest.Value,
            
            // Propriétés D&D par défaut ou depuis le dictionnaire
            WeaponType = GetStringProperty(equipmentRequest.SpecializedProperties, "WeaponType", ""),
            Damage = GetStringProperty(equipmentRequest.SpecializedProperties, "Damage", ""),
            DamageType = GetStringProperty(equipmentRequest.SpecializedProperties, "DamageType", ""),
            AttackBonus = GetIntProperty(equipmentRequest.SpecializedProperties, "AttackBonus", 0),
            Properties = GetStringProperty(equipmentRequest.SpecializedProperties, "Properties", ""),
            ArmorClass = GetIntProperty(equipmentRequest.SpecializedProperties, "ArmorClass", 0),
            MaxDexBonus = GetIntProperty(equipmentRequest.SpecializedProperties, "MaxDexBonus", -1),
            StealthDisadvantage = GetBoolProperty(equipmentRequest.SpecializedProperties, "StealthDisadvantage", false),
            StrengthRequirement = GetIntProperty(equipmentRequest.SpecializedProperties, "StrengthRequirement", 0),
            IsWeapon = GetBoolProperty(equipmentRequest.SpecializedProperties, "IsWeapon", false),
            IsArmor = GetBoolProperty(equipmentRequest.SpecializedProperties, "IsArmor", false),
            IsShield = GetBoolProperty(equipmentRequest.SpecializedProperties, "IsShield", false),
            IsMagical = GetBoolProperty(equipmentRequest.SpecializedProperties, "IsMagical", false),
            RequiresAttunement = GetBoolProperty(equipmentRequest.SpecializedProperties, "RequiresAttunement", false),
            Rarity = GetStringProperty(equipmentRequest.SpecializedProperties, "Rarity", "Common"),
            MagicalProperties = GetStringProperty(equipmentRequest.SpecializedProperties, "MagicalProperties", null),
            Charges = GetNullableIntProperty(equipmentRequest.SpecializedProperties, "Charges"),
            IsConsumable = GetBoolProperty(equipmentRequest.SpecializedProperties, "IsConsumable", false),
            Effect = GetStringProperty(equipmentRequest.SpecializedProperties, "Effect", null)
        };

        if (equipmentRequest.Tags != null)
        {
            equipment.SetTags(equipmentRequest.Tags);
        }

        this.context.EquipmentDnd.Add(equipment);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("D&D equipment {Name} created with ID {Id} for user {UserId}", 
            equipment.Name, equipment.Id, userId);

        return this.MapToView(equipment);
    }

    public async Task<IEquipmentView> UpdateEquipment(EquipmentRequest equipmentRequest, int equipmentId, int userId)
    {
        this.logger.LogInformation("Updating D&D equipment {EquipmentId} for user {UserId}", equipmentId, userId);

        var existingEquipment = await this.context.EquipmentDnd
            .FirstOrDefaultAsync(e => e.Id == equipmentId && e.IsActive);

        if (existingEquipment == null)
            throw new InvalidOperationException($"Equipment with ID {equipmentId} not found");

        if (existingEquipment.CreatedByUserId != userId || existingEquipment.IsPublic)
            throw new BusinessException("You don't have permission to update this equipment");

        // Validation métier
        if (string.IsNullOrWhiteSpace(equipmentRequest.Name))
            throw new BusinessException("Le nom de l'équipement est obligatoire.");

        if (string.IsNullOrWhiteSpace(equipmentRequest.Description))
            throw new BusinessException("La description de l'équipement est obligatoire.");

        // Vérifier l'unicité du nom (sauf pour l'équipement actuel)
        var duplicateEquipment = await this.context.EquipmentDnd
            .FirstOrDefaultAsync(e => e.Name == equipmentRequest.Name && 
                                    e.CreatedByUserId == userId && 
                                    e.Id != equipmentId && 
                                    e.IsActive);

        if (duplicateEquipment != null)
            throw new BusinessException($"Vous avez déjà un autre équipement D&D nommé '{equipmentRequest.Name}'.");

        // Mise à jour des propriétés
        existingEquipment.Name = equipmentRequest.Name;
        existingEquipment.Description = equipmentRequest.Description;
        existingEquipment.ImageUrl = equipmentRequest.ImageUrl;
        existingEquipment.Category = equipmentRequest.Category;
        existingEquipment.Weight = equipmentRequest.Weight;
        existingEquipment.Value = equipmentRequest.Value;
        
        // Mise à jour des propriétés D&D
        if (equipmentRequest.SpecializedProperties != null)
        {
            existingEquipment.WeaponType = GetStringProperty(equipmentRequest.SpecializedProperties, "WeaponType", existingEquipment.WeaponType);
            existingEquipment.Damage = GetStringProperty(equipmentRequest.SpecializedProperties, "Damage", existingEquipment.Damage);
            existingEquipment.DamageType = GetStringProperty(equipmentRequest.SpecializedProperties, "DamageType", existingEquipment.DamageType);
            existingEquipment.AttackBonus = GetIntProperty(equipmentRequest.SpecializedProperties, "AttackBonus", existingEquipment.AttackBonus);
            existingEquipment.Properties = GetStringProperty(equipmentRequest.SpecializedProperties, "Properties", existingEquipment.Properties);
            existingEquipment.ArmorClass = GetIntProperty(equipmentRequest.SpecializedProperties, "ArmorClass", existingEquipment.ArmorClass);
            existingEquipment.MaxDexBonus = GetIntProperty(equipmentRequest.SpecializedProperties, "MaxDexBonus", existingEquipment.MaxDexBonus);
            existingEquipment.StealthDisadvantage = GetBoolProperty(equipmentRequest.SpecializedProperties, "StealthDisadvantage", existingEquipment.StealthDisadvantage);
            existingEquipment.StrengthRequirement = GetIntProperty(equipmentRequest.SpecializedProperties, "StrengthRequirement", existingEquipment.StrengthRequirement);
            existingEquipment.IsWeapon = GetBoolProperty(equipmentRequest.SpecializedProperties, "IsWeapon", existingEquipment.IsWeapon);
            existingEquipment.IsArmor = GetBoolProperty(equipmentRequest.SpecializedProperties, "IsArmor", existingEquipment.IsArmor);
            existingEquipment.IsShield = GetBoolProperty(equipmentRequest.SpecializedProperties, "IsShield", existingEquipment.IsShield);
            existingEquipment.IsMagical = GetBoolProperty(equipmentRequest.SpecializedProperties, "IsMagical", existingEquipment.IsMagical);
            existingEquipment.RequiresAttunement = GetBoolProperty(equipmentRequest.SpecializedProperties, "RequiresAttunement", existingEquipment.RequiresAttunement);
            existingEquipment.Rarity = GetStringProperty(equipmentRequest.SpecializedProperties, "Rarity", existingEquipment.Rarity);
            existingEquipment.MagicalProperties = GetStringProperty(equipmentRequest.SpecializedProperties, "MagicalProperties", existingEquipment.MagicalProperties);
            existingEquipment.Charges = GetNullableIntProperty(equipmentRequest.SpecializedProperties, "Charges") ?? existingEquipment.Charges;
            existingEquipment.IsConsumable = GetBoolProperty(equipmentRequest.SpecializedProperties, "IsConsumable", existingEquipment.IsConsumable);
            existingEquipment.Effect = GetStringProperty(equipmentRequest.SpecializedProperties, "Effect", existingEquipment.Effect);
        }

        if (equipmentRequest.Tags != null)
        {
            existingEquipment.SetTags(equipmentRequest.Tags);
        }

        existingEquipment.UpdatedAt = DateTime.UtcNow;

        await this.context.SaveChangesAsync();

        this.logger.LogInformation("D&D equipment {EquipmentId} updated successfully for user {UserId}", equipmentId, userId);

        return this.MapToView(existingEquipment);
    }

    public async Task DeleteEquipment(int equipmentId, int userId)
    {
        this.logger.LogInformation("Deleting D&D equipment {EquipmentId} for user {UserId}", equipmentId, userId);

        var equipment = await this.context.EquipmentDnd
            .FirstOrDefaultAsync(e => e.Id == equipmentId && e.IsActive);

        if (equipment == null)
            throw new InvalidOperationException($"Equipment with ID {equipmentId} not found");

        if (equipment.CreatedByUserId != userId || equipment.IsPublic)
            throw new BusinessException("You don't have permission to delete this equipment");

        // Soft delete
        equipment.IsActive = false;
        equipment.UpdatedAt = DateTime.UtcNow;

        await this.context.SaveChangesAsync();

        this.logger.LogInformation("D&D equipment {EquipmentId} deleted successfully for user {UserId}", equipmentId, userId);
    }

    public async Task<IEnumerable<IEquipmentView>> SearchEquipments(string searchText, int userId, GameType gameType)
    {
        this.logger.LogInformation("Searching D&D equipment with text '{SearchText}' for user {UserId}", searchText, userId);

        if (string.IsNullOrWhiteSpace(searchText))
            return await this.GetAllEquipmentsByUserId(userId, gameType);

        var searchTermLower = searchText.ToLower();

        var equipment = await this.context.EquipmentDnd
            .Where(e => e.IsActive &&
                       (e.IsPublic || e.CreatedByUserId == userId) &&
                       (e.Name.ToLower().Contains(searchTermLower) || 
                        e.Description.ToLower().Contains(searchTermLower) ||
                        e.Category.ToLower().Contains(searchTermLower) ||
                        e.WeaponType.ToLower().Contains(searchTermLower) ||
                        e.DamageType.ToLower().Contains(searchTermLower) ||
                        (e.Tags != null && e.Tags.ToLower().Contains(searchTermLower))))
            .OrderBy(e => e.Category)
            .ThenBy(e => e.Name)
            .ToListAsync();

        return equipment.Select(this.MapToView).ToList();
    }

    public async Task<IEnumerable<IEquipmentView>> GetEquipmentsByCategory(string category, int userId, GameType gameType)
    {
        this.logger.LogInformation("Getting D&D equipment by category {Category} for user {UserId}", category, userId);

        var equipment = await this.context.EquipmentDnd
            .Where(e => e.Category == category && 
                       e.IsActive &&
                       (e.IsPublic || e.CreatedByUserId == userId))
            .OrderBy(e => e.Name)
            .ToListAsync();

        return equipment.Select(this.MapToView).ToList();
    }

    private EquipmentDndView MapToView(EquipmentDnd equipment)
    {
        return new EquipmentDndView
        {
            Id = equipment.Id,
            Name = equipment.Name,
            Description = equipment.Description,
            ImageUrl = equipment.ImageUrl,
            IsPublic = equipment.IsPublic,
            Source = equipment.Source.ToString(),
            CreatedByName = equipment.CreatedBy?.UserName,
            Tags = equipment.GetTags(),
            Category = equipment.Category,
            Weight = equipment.Weight,
            Value = equipment.Value,
            CreatedAt = equipment.CreatedAt,
            UpdatedAt = equipment.UpdatedAt,
            WeaponType = equipment.WeaponType,
            Damage = equipment.Damage,
            DamageType = equipment.DamageType,
            AttackBonus = equipment.AttackBonus,
            Properties = equipment.Properties,
            ArmorClass = equipment.ArmorClass,
            MaxDexBonus = equipment.MaxDexBonus,
            StealthDisadvantage = equipment.StealthDisadvantage,
            StrengthRequirement = equipment.StrengthRequirement,
            IsWeapon = equipment.IsWeapon,
            IsArmor = equipment.IsArmor,
            IsShield = equipment.IsShield,
            IsMagical = equipment.IsMagical,
            RequiresAttunement = equipment.RequiresAttunement,
            Rarity = equipment.Rarity,
            MagicalProperties = equipment.MagicalProperties,
            Charges = equipment.Charges,
            IsConsumable = equipment.IsConsumable,
            Effect = equipment.Effect
        };
    }

    // Méthodes utilitaires pour extraire les propriétés du dictionnaire
    private static string GetStringProperty(Dictionary<string, object>? properties, string key, string? defaultValue)
    {
        if (properties?.TryGetValue(key, out var value) == true)
            return value?.ToString() ?? defaultValue ?? "";
        return defaultValue ?? "";
    }

    private static int GetIntProperty(Dictionary<string, object>? properties, string key, int defaultValue)
    {
        if (properties?.TryGetValue(key, out var value) == true && int.TryParse(value?.ToString(), out var intValue))
            return intValue;
        return defaultValue;
    }

    private static bool GetBoolProperty(Dictionary<string, object>? properties, string key, bool defaultValue)
    {
        if (properties?.TryGetValue(key, out var value) == true && bool.TryParse(value?.ToString(), out var boolValue))
            return boolValue;
        return defaultValue;
    }

    private static int? GetNullableIntProperty(Dictionary<string, object>? properties, string key)
    {
        if (properties?.TryGetValue(key, out var value) == true && int.TryParse(value?.ToString(), out var intValue))
            return intValue;
        return null;
    }
}