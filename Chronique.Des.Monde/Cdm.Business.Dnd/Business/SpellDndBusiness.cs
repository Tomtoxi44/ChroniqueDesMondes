using Cdm.Data.Dnd;
using Cdm.Data.Dnd.Models;
using Cdm.Data.Models;
using Cdm.Business.Dnd.Models.Spells;
using Cdm.Common;
using Cdm.Common.Enums;
using Cmd.Abstraction.Spells;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cdm.Business.Dnd.Business;

/// <summary>
/// Service métier spécialisé pour la gestion des sorts D&D
/// Implémente ISpellBusiness pour l'injection par clé, suit le pattern CharacterDndBusiness
/// </summary>
public class SpellDndBusiness : ISpellBusiness
{
    private readonly DndDbContext context;
    private readonly ILogger<SpellDndBusiness> logger;

    public SpellDndBusiness(DndDbContext context, ILogger<SpellDndBusiness> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<IEnumerable<ISpellView>> GetAllSpellsByUserId(int userId, GameType gameType)
    {
        this.logger.LogInformation("Getting all D&D spells for user {UserId}", userId);

        var spells = await this.context.SpellsDnd
            .Where(s => s.IsActive &&
                       (s.IsPublic || s.CreatedByUserId == userId))
            .OrderBy(s => s.Level)
            .ThenBy(s => s.School)
            .ThenBy(s => s.Name)
            .ToListAsync();

        return spells.Select(this.MapToView).ToList();
    }

    public async Task<ISpellView> GetSpellById(int spellId, int userId)
    {
        this.logger.LogInformation("Getting D&D spell {SpellId} for user {UserId}", spellId, userId);

        var spell = await this.context.SpellsDnd
            .Include(s => s.CreatedBy)
            .FirstOrDefaultAsync(s => s.Id == spellId && s.IsActive);

        if (spell == null)
            throw new InvalidOperationException($"Spell with ID {spellId} not found");

        // Vérifier les droits d'accès
        if (!spell.IsPublic && spell.CreatedByUserId != userId)
            throw new BusinessException("You don't have access to this spell");

        return this.MapToView(spell);
    }

    public async Task<ISpellView> CreateSpell(SpellRequest spellRequest, int userId)
    {
        this.logger.LogInformation("Creating D&D spell {Name} for user {UserId}", spellRequest.Name, userId);

        // Validation métier D&D
        if (string.IsNullOrWhiteSpace(spellRequest.Name))
            throw new BusinessException("Le nom du sort est obligatoire.");

        if (string.IsNullOrWhiteSpace(spellRequest.Description))
            throw new BusinessException("La description du sort est obligatoire.");

        // Vérifier l'unicité du nom pour cet utilisateur
        var existingSpell = await this.context.SpellsDnd
            .FirstOrDefaultAsync(s => s.Name == spellRequest.Name && 
                                    s.CreatedByUserId == userId && 
                                    s.IsActive);

        if (existingSpell != null)
            throw new BusinessException($"Vous avez déjà un sort D&D nommé '{spellRequest.Name}'.");

        // Extraire les propriétés D&D depuis le dictionnaire
        var spell = new SpellDnd
        {
            Name = spellRequest.Name,
            Description = spellRequest.Description,
            ImageUrl = spellRequest.ImageUrl,
            GameType = GameType.DnD,
            CreatedByUserId = userId,
            IsPublic = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            
            // Propriétés D&D par défaut
            School = GetStringProperty(spellRequest.SpecializedProperties, "School", "Évocation"),
            Level = GetIntProperty(spellRequest.SpecializedProperties, "Level", 1),
            CastingTime = GetStringProperty(spellRequest.SpecializedProperties, "CastingTime", "1 action"),
            Range = GetStringProperty(spellRequest.SpecializedProperties, "Range", "Contact"),
            Duration = GetStringProperty(spellRequest.SpecializedProperties, "Duration", "Instantané"),
            Components = GetStringProperty(spellRequest.SpecializedProperties, "Components", "V, S"),
            AttackRoll = GetStringProperty(spellRequest.SpecializedProperties, "AttackRoll", ""),
            Damage = GetStringProperty(spellRequest.SpecializedProperties, "Damage", ""),
            SavingThrow = GetStringProperty(spellRequest.SpecializedProperties, "SavingThrow", ""),
            IsRitual = GetBoolProperty(spellRequest.SpecializedProperties, "IsRitual", false),
            RequiresConcentration = GetBoolProperty(spellRequest.SpecializedProperties, "RequiresConcentration", false),
            MaterialComponent = GetStringProperty(spellRequest.SpecializedProperties, "MaterialComponent", ""),
            HigherLevelDamage = GetNullableIntProperty(spellRequest.SpecializedProperties, "HigherLevelDamage")
        };

        if (spellRequest.Tags != null)
        {
            spell.SetTags(spellRequest.Tags);
        }

        this.context.SpellsDnd.Add(spell);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("D&D spell {Name} created with ID {Id} for user {UserId}", 
            spell.Name, spell.Id, userId);

        return this.MapToView(spell);
    }

    public async Task<ISpellView> UpdateSpell(SpellRequest spellRequest, int spellId, int userId)
    {
        this.logger.LogInformation("Updating D&D spell {SpellId} for user {UserId}", spellId, userId);

        var existingSpell = await this.context.SpellsDnd
            .FirstOrDefaultAsync(s => s.Id == spellId && s.IsActive);

        if (existingSpell == null)
            throw new InvalidOperationException($"Spell with ID {spellId} not found");

        if (existingSpell.CreatedByUserId != userId || existingSpell.IsPublic)
            throw new BusinessException("You don't have permission to update this spell");

        // Validation métier
        if (string.IsNullOrWhiteSpace(spellRequest.Name))
            throw new BusinessException("Le nom du sort est obligatoire.");

        if (string.IsNullOrWhiteSpace(spellRequest.Description))
            throw new BusinessException("La description du sort est obligatoire.");

        // Vérifier l'unicité du nom (sauf pour le sort actuel)
        var duplicateSpell = await this.context.SpellsDnd
            .FirstOrDefaultAsync(s => s.Name == spellRequest.Name && 
                                    s.CreatedByUserId == userId && 
                                    s.Id != spellId && 
                                    s.IsActive);

        if (duplicateSpell != null)
            throw new BusinessException($"Vous avez déjà un autre sort D&D nommé '{spellRequest.Name}'.");

        // Mise à jour des propriétés
        existingSpell.Name = spellRequest.Name;
        existingSpell.Description = spellRequest.Description;
        existingSpell.ImageUrl = spellRequest.ImageUrl;
        
        // Mise à jour des propriétés D&D
        if (spellRequest.SpecializedProperties != null)
        {
            existingSpell.School = GetStringProperty(spellRequest.SpecializedProperties, "School", existingSpell.School);
            existingSpell.Level = GetIntProperty(spellRequest.SpecializedProperties, "Level", existingSpell.Level);
            existingSpell.CastingTime = GetStringProperty(spellRequest.SpecializedProperties, "CastingTime", existingSpell.CastingTime);
            existingSpell.Range = GetStringProperty(spellRequest.SpecializedProperties, "Range", existingSpell.Range);
            existingSpell.Duration = GetStringProperty(spellRequest.SpecializedProperties, "Duration", existingSpell.Duration);
            existingSpell.Components = GetStringProperty(spellRequest.SpecializedProperties, "Components", existingSpell.Components);
            existingSpell.AttackRoll = GetStringProperty(spellRequest.SpecializedProperties, "AttackRoll", existingSpell.AttackRoll);
            existingSpell.Damage = GetStringProperty(spellRequest.SpecializedProperties, "Damage", existingSpell.Damage);
            existingSpell.SavingThrow = GetStringProperty(spellRequest.SpecializedProperties, "SavingThrow", existingSpell.SavingThrow);
            existingSpell.IsRitual = GetBoolProperty(spellRequest.SpecializedProperties, "IsRitual", existingSpell.IsRitual);
            existingSpell.RequiresConcentration = GetBoolProperty(spellRequest.SpecializedProperties, "RequiresConcentration", existingSpell.RequiresConcentration);
            existingSpell.MaterialComponent = GetStringProperty(spellRequest.SpecializedProperties, "MaterialComponent", existingSpell.MaterialComponent);
            existingSpell.HigherLevelDamage = GetNullableIntProperty(spellRequest.SpecializedProperties, "HigherLevelDamage") ?? existingSpell.HigherLevelDamage;
        }

        if (spellRequest.Tags != null)
        {
            existingSpell.SetTags(spellRequest.Tags);
        }

        existingSpell.UpdatedAt = DateTime.UtcNow;

        await this.context.SaveChangesAsync();

        this.logger.LogInformation("D&D spell {SpellId} updated successfully for user {UserId}", spellId, userId);

        return this.MapToView(existingSpell);
    }

    public async Task DeleteSpell(int spellId, int userId)
    {
        this.logger.LogInformation("Deleting D&D spell {SpellId} for user {UserId}", spellId, userId);

        var spell = await this.context.SpellsDnd
            .FirstOrDefaultAsync(s => s.Id == spellId && s.IsActive);

        if (spell == null)
            throw new InvalidOperationException($"Spell with ID {spellId} not found");

        if (spell.CreatedByUserId != userId || spell.IsPublic)
            throw new BusinessException("You don't have permission to delete this spell");

        // Soft delete
        spell.IsActive = false;
        spell.UpdatedAt = DateTime.UtcNow;

        await this.context.SaveChangesAsync();

        this.logger.LogInformation("D&D spell {SpellId} deleted successfully for user {UserId}", spellId, userId);
    }

    public async Task<IEnumerable<ISpellView>> SearchSpells(string searchText, int userId, GameType gameType)
    {
        this.logger.LogInformation("Searching D&D spells with text '{SearchText}' for user {UserId}", searchText, userId);

        if (string.IsNullOrWhiteSpace(searchText))
            return await this.GetAllSpellsByUserId(userId, gameType);

        var searchTermLower = searchText.ToLower();

        var spells = await this.context.SpellsDnd
            .Where(s => s.IsActive &&
                       (s.IsPublic || s.CreatedByUserId == userId) &&
                       (s.Name.ToLower().Contains(searchTermLower) || 
                        s.Description.ToLower().Contains(searchTermLower) ||
                        s.School.ToLower().Contains(searchTermLower) ||
                        (s.Tags != null && s.Tags.ToLower().Contains(searchTermLower))))
            .OrderBy(s => s.Level)
            .ThenBy(s => s.School)
            .ThenBy(s => s.Name)
            .ToListAsync();

        return spells.Select(this.MapToView).ToList();
    }

    private SpellDndView MapToView(SpellDnd spell)
    {
        return new SpellDndView
        {
            Id = spell.Id,
            Name = spell.Name,
            Description = spell.Description,
            ImageUrl = spell.ImageUrl,
            IsPublic = spell.IsPublic,
            Source = spell.Source.ToString(),
            CreatedByName = spell.CreatedBy?.UserName,
            Tags = spell.GetTags(),
            CreatedAt = spell.CreatedAt,
            UpdatedAt = spell.UpdatedAt,
            School = spell.School,
            Level = spell.Level,
            CastingTime = spell.CastingTime,
            Range = spell.Range,
            Duration = spell.Duration,
            Components = spell.Components,
            AttackRoll = spell.AttackRoll,
            Damage = spell.Damage,
            SavingThrow = spell.SavingThrow,
            IsRitual = spell.IsRitual,
            RequiresConcentration = spell.RequiresConcentration,
            MaterialComponent = spell.MaterialComponent,
            HigherLevelDamage = spell.HigherLevelDamage
        };
    }

    // Méthodes utilitaires pour extraire les propriétés du dictionnaire
    private static string GetStringProperty(Dictionary<string, object>? properties, string key, string defaultValue)
    {
        if (properties?.TryGetValue(key, out var value) == true)
            return value?.ToString() ?? defaultValue;
        return defaultValue;
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