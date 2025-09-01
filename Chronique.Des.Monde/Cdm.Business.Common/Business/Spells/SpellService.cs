using Cdm.Data;
using Cdm.Data.Models;
using Cdm.Common;
using Cdm.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cdm.Business.Common.Business.Spells;

/// <summary>
/// Service métier pour la gestion des sorts (générique)
/// Suit le pattern des autres services du projet avec validation des permissions
/// </summary>
public class SpellService : ISpellService
{
    private readonly AppDbContext dbContext;
    private readonly ILogger<SpellService> logger;

    public SpellService(AppDbContext dbContext, ILogger<SpellService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<List<ASpell>> GetOfficialSpellsAsync(GameType gameType)
    {
        this.logger.LogInformation("Getting official spells for game type {GameType}", gameType);

        return await this.dbContext.Spells
            .Where(s => s.GameType == gameType && 
                       s.CreatedByUserId == 0 && 
                       s.IsPublic && 
                       s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<List<ASpell>> GetUserPrivateSpellsAsync(int userId, GameType gameType)
    {
        this.logger.LogInformation("Getting private spells for user {UserId} and game type {GameType}", userId, gameType);

        return await this.dbContext.Spells
            .Where(s => s.GameType == gameType && 
                       s.CreatedByUserId == userId && 
                       !s.IsPublic && 
                       s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<List<ASpell>> GetAvailableSpellsAsync(int userId, GameType gameType)
    {
        this.logger.LogInformation("Getting all available spells for user {UserId} and game type {GameType}", userId, gameType);

        return await this.dbContext.Spells
            .Where(s => s.GameType == gameType && 
                       s.IsActive &&
                       (s.IsPublic || s.CreatedByUserId == userId))
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<ASpell?> GetSpellByIdAsync(int spellId)
    {
        this.logger.LogInformation("Getting spell by ID {SpellId}", spellId);

        return await this.dbContext.Spells
            .Include(s => s.CreatedBy)
            .FirstOrDefaultAsync(s => s.Id == spellId && s.IsActive);
    }

    public async Task<bool> CanUserModifySpellAsync(int userId, int spellId)
    {
        var spell = await this.dbContext.Spells
            .FirstOrDefaultAsync(s => s.Id == spellId && s.IsActive);

        if (spell == null)
            return false;

        // Seuls les sorts privés peuvent être modifiés, et seulement par leur créateur
        return spell.CreatedByUserId == userId && !spell.IsPublic;
    }

    public async Task<bool> CanUserViewSpellAsync(int userId, int spellId)
    {
        var spell = await this.dbContext.Spells
            .FirstOrDefaultAsync(s => s.Id == spellId && s.IsActive);

        if (spell == null)
            return false;

        // Les sorts publics sont visibles par tous, les privés seulement par leur créateur
        return spell.IsPublic || spell.CreatedByUserId == userId;
    }

    public async Task<ASpell> CreatePrivateSpellAsync(ASpell spell, int userId)
    {
        this.logger.LogInformation("Creating private spell {Name} for user {UserId}", spell.Name, userId);

        // Validation métier
        if (string.IsNullOrWhiteSpace(spell.Name))
            throw new BusinessException("Le nom du sort est obligatoire.");

        if (string.IsNullOrWhiteSpace(spell.Description))
            throw new BusinessException("La description du sort est obligatoire.");

        // Vérifier l'unicité du nom pour cet utilisateur et ce type de jeu
        var existingSpell = await this.dbContext.Spells
            .FirstOrDefaultAsync(s => s.Name == spell.Name && 
                                    s.GameType == spell.GameType && 
                                    s.CreatedByUserId == userId && 
                                    s.IsActive);

        if (existingSpell != null)
            throw new BusinessException($"Vous avez déjà un sort nommé '{spell.Name}' pour ce type de jeu.");

        // Configuration du sort privé
        spell.CreatedByUserId = userId;
        spell.IsPublic = false; // Les sorts utilisateur sont toujours privés
        spell.IsActive = true;
        spell.CreatedAt = DateTime.UtcNow;
        spell.UpdatedAt = null;

        this.dbContext.Spells.Add(spell);
        await this.dbContext.SaveChangesAsync();

        this.logger.LogInformation("Private spell {Name} created with ID {Id} for user {UserId}", 
            spell.Name, spell.Id, userId);

        return spell;
    }

    public async Task<ASpell> UpdateSpellAsync(ASpell spell, int userId)
    {
        this.logger.LogInformation("Updating spell {SpellId} for user {UserId}", spell.Id, userId);

        var existingSpell = await this.dbContext.Spells
            .FirstOrDefaultAsync(s => s.Id == spell.Id && s.IsActive);

        if (existingSpell == null)
            throw new BusinessException("Sort introuvable.");

        if (!await this.CanUserModifySpellAsync(userId, spell.Id))
            throw new BusinessException("Vous n'avez pas le droit de modifier ce sort.");

        // Validation métier
        if (string.IsNullOrWhiteSpace(spell.Name))
            throw new BusinessException("Le nom du sort est obligatoire.");

        if (string.IsNullOrWhiteSpace(spell.Description))
            throw new BusinessException("La description du sort est obligatoire.");

        // Vérifier l'unicité du nom (sauf pour le sort actuel)
        var duplicateSpell = await this.dbContext.Spells
            .FirstOrDefaultAsync(s => s.Name == spell.Name && 
                                    s.GameType == spell.GameType && 
                                    s.CreatedByUserId == userId && 
                                    s.Id != spell.Id && 
                                    s.IsActive);

        if (duplicateSpell != null)
            throw new BusinessException($"Vous avez déjà un autre sort nommé '{spell.Name}' pour ce type de jeu.");

        // Mise à jour des propriétés modifiables
        existingSpell.Name = spell.Name;
        existingSpell.Description = spell.Description;
        existingSpell.ImageUrl = spell.ImageUrl;
        existingSpell.Tags = spell.Tags;
        existingSpell.UpdatedAt = DateTime.UtcNow;

        await this.dbContext.SaveChangesAsync();

        this.logger.LogInformation("Spell {SpellId} updated successfully for user {UserId}", spell.Id, userId);

        return existingSpell;
    }

    public async Task DeleteSpellAsync(int spellId, int userId)
    {
        this.logger.LogInformation("Deleting spell {SpellId} for user {UserId}", spellId, userId);

        var spell = await this.dbContext.Spells
            .FirstOrDefaultAsync(s => s.Id == spellId && s.IsActive);

        if (spell == null)
            throw new BusinessException("Sort introuvable.");

        if (!await this.CanUserModifySpellAsync(userId, spellId))
            throw new BusinessException("Vous n'avez pas le droit de supprimer ce sort.");

        // Soft delete
        spell.IsActive = false;
        spell.UpdatedAt = DateTime.UtcNow;

        await this.dbContext.SaveChangesAsync();

        this.logger.LogInformation("Spell {SpellId} deleted successfully for user {UserId}", spellId, userId);
    }

    public async Task<List<ASpell>> SearchSpellsAsync(string searchText, GameType gameType, int userId)
    {
        this.logger.LogInformation("Searching spells with text '{SearchText}' for game type {GameType} and user {UserId}", 
            searchText, gameType, userId);

        if (string.IsNullOrWhiteSpace(searchText))
            return await this.GetAvailableSpellsAsync(userId, gameType);

        var searchTermLower = searchText.ToLower();

        return await this.dbContext.Spells
            .Where(s => s.GameType == gameType && 
                       s.IsActive &&
                       (s.IsPublic || s.CreatedByUserId == userId) &&
                       (s.Name.ToLower().Contains(searchTermLower) || 
                        s.Description.ToLower().Contains(searchTermLower) ||
                        (s.Tags != null && s.Tags.ToLower().Contains(searchTermLower))))
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
}