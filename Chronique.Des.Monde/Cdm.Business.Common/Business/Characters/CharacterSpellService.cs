using Cdm.Data.Dnd;
using Cdm.Data.Common.Models;
using Cdm.Common;
using Cdm.Common.Enums;
using Cmd.Abstraction.Characters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cdm.Business.Common.Business.Characters;

/// <summary>
/// Service pour l'attribution et la gestion des sorts d'un personnage
/// </summary>
public class CharacterSpellService : ICharacterSpellService
{
    private readonly DndDbContext context;
    private readonly ILogger<CharacterSpellService> logger;

    public CharacterSpellService(DndDbContext context, ILogger<CharacterSpellService> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<IEnumerable<CharacterSpellDto>> GetCharacterSpellsAsync(int characterId)
    {
        this.logger.LogInformation("Getting spells for character {CharacterId}", characterId);

        var characterSpells = await this.context.CharacterSpells
            .Where(cs => cs.CharacterId == characterId)
            .Join(this.context.SpellsDnd,
                cs => cs.SpellId,
                s => s.Id,
                (cs, s) => new { CharacterSpell = cs, Spell = s })
            .OrderBy(x => x.Spell.Level)
            .ThenBy(x => x.Spell.School)
            .ThenBy(x => x.Spell.Name)
            .ToListAsync();

        return characterSpells.Select(x => new CharacterSpellDto
        {
            Id = x.CharacterSpell.Id,
            CharacterId = x.CharacterSpell.CharacterId,
            SpellId = x.CharacterSpell.SpellId,
            SpellName = x.Spell.Name,
            SpellDescription = x.Spell.Description,
            SpellGameType = x.Spell.GameType,
            LearnedAt = x.CharacterSpell.LearnedAt,
            Notes = x.CharacterSpell.Notes,
            IsPrepared = x.CharacterSpell.IsPrepared,
            SpellSlotLevel = x.CharacterSpell.SpellSlotLevel,
            CustomName = x.CharacterSpell.CustomName,
            School = x.Spell.School,
            Level = x.Spell.Level,
            CastingTime = x.Spell.CastingTime,
            Range = x.Spell.Range,
            Duration = x.Spell.Duration,
            IsRitual = x.Spell.IsRitual,
            RequiresConcentration = x.Spell.RequiresConcentration
        });
    }

    public async Task<CharacterSpellDto> AddSpellToCharacterAsync(int characterId, int spellId, string? notes = null)
    {
        this.logger.LogInformation("Adding spell {SpellId} to character {CharacterId}", spellId, characterId);

        // Vérifier que le personnage peut apprendre ce sort
        if (!await this.CanCharacterLearnSpellAsync(characterId, spellId))
        {
            throw new BusinessException("Ce personnage ne peut pas apprendre ce sort (incompatibilité de système de jeu).");
        }

        // Vérifier que le personnage n'a pas déjà ce sort
        var existingSpell = await this.context.CharacterSpells
            .FirstOrDefaultAsync(cs => cs.CharacterId == characterId && cs.SpellId == spellId);

        if (existingSpell != null)
        {
            throw new BusinessException("Ce personnage connaît déjà ce sort.");
        }

        // Ajouter le sort au personnage
        var characterSpell = new CharacterSpells
        {
            CharacterId = characterId,
            SpellId = spellId,
            LearnedAt = DateTime.UtcNow,
            Notes = notes,
            IsPrepared = false // Par défaut non préparé
        };

        this.context.CharacterSpells.Add(characterSpell);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("Spell {SpellId} successfully added to character {CharacterId}", spellId, characterId);

        // Retourner le sort avec ses détails
        var spell = await this.context.SpellsDnd.FindAsync(spellId);
        return new CharacterSpellDto
        {
            Id = characterSpell.Id,
            CharacterId = characterSpell.CharacterId,
            SpellId = characterSpell.SpellId,
            SpellName = spell?.Name ?? "",
            SpellDescription = spell?.Description ?? "",
            SpellGameType = spell?.GameType ?? GameType.Generic,
            LearnedAt = characterSpell.LearnedAt,
            Notes = characterSpell.Notes,
            IsPrepared = characterSpell.IsPrepared,
            SpellSlotLevel = characterSpell.SpellSlotLevel,
            CustomName = characterSpell.CustomName,
            School = spell?.School,
            Level = spell?.Level,
            CastingTime = spell?.CastingTime,
            Range = spell?.Range,
            Duration = spell?.Duration,
            IsRitual = spell?.IsRitual ?? false,
            RequiresConcentration = spell?.RequiresConcentration ?? false
        };
    }

    public async Task RemoveSpellFromCharacterAsync(int characterId, int spellId)
    {
        this.logger.LogInformation("Removing spell {SpellId} from character {CharacterId}", spellId, characterId);

        var characterSpell = await this.context.CharacterSpells
            .FirstOrDefaultAsync(cs => cs.CharacterId == characterId && cs.SpellId == spellId);

        if (characterSpell == null)
        {
            throw new InvalidOperationException("Ce personnage ne connaît pas ce sort.");
        }

        this.context.CharacterSpells.Remove(characterSpell);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("Spell {SpellId} successfully removed from character {CharacterId}", spellId, characterId);
    }

    public async Task<CharacterSpellDto> UpdateCharacterSpellAsync(int characterId, int spellId, UpdateCharacterSpellRequest request)
    {
        this.logger.LogInformation("Updating spell {SpellId} for character {CharacterId}", spellId, characterId);

        var characterSpell = await this.context.CharacterSpells
            .FirstOrDefaultAsync(cs => cs.CharacterId == characterId && cs.SpellId == spellId);

        if (characterSpell == null)
        {
            throw new InvalidOperationException("Ce personnage ne connaît pas ce sort.");
        }

        // Mettre à jour les propriétés
        if (request.Notes != null)
            characterSpell.Notes = request.Notes;

        if (request.IsPrepared.HasValue)
            characterSpell.IsPrepared = request.IsPrepared.Value;

        if (request.SpellSlotLevel.HasValue)
            characterSpell.SpellSlotLevel = request.SpellSlotLevel.Value;

        if (request.CustomName != null)
            characterSpell.CustomName = request.CustomName;

        await this.context.SaveChangesAsync();

        // Retourner le sort mis à jour
        var spell = await this.context.SpellsDnd.FindAsync(spellId);
        return new CharacterSpellDto
        {
            Id = characterSpell.Id,
            CharacterId = characterSpell.CharacterId,
            SpellId = characterSpell.SpellId,
            SpellName = spell?.Name ?? "",
            SpellDescription = spell?.Description ?? "",
            SpellGameType = spell?.GameType ?? GameType.Generic,
            LearnedAt = characterSpell.LearnedAt,
            Notes = characterSpell.Notes,
            IsPrepared = characterSpell.IsPrepared,
            SpellSlotLevel = characterSpell.SpellSlotLevel,
            CustomName = characterSpell.CustomName,
            School = spell?.School,
            Level = spell?.Level,
            CastingTime = spell?.CastingTime,
            Range = spell?.Range,
            Duration = spell?.Duration,
            IsRitual = spell?.IsRitual ?? false,
            RequiresConcentration = spell?.RequiresConcentration ?? false
        };
    }

    public async Task<IEnumerable<CharacterSpellDto>> GetPreparedSpellsAsync(int characterId)
    {
        this.logger.LogInformation("Getting prepared spells for character {CharacterId}", characterId);

        var preparedSpells = await this.context.CharacterSpells
            .Where(cs => cs.CharacterId == characterId && cs.IsPrepared)
            .Join(this.context.SpellsDnd,
                cs => cs.SpellId,
                s => s.Id,
                (cs, s) => new { CharacterSpell = cs, Spell = s })
            .OrderBy(x => x.Spell.Level)
            .ThenBy(x => x.Spell.Name)
            .ToListAsync();

        return preparedSpells.Select(x => new CharacterSpellDto
        {
            Id = x.CharacterSpell.Id,
            CharacterId = x.CharacterSpell.CharacterId,
            SpellId = x.CharacterSpell.SpellId,
            SpellName = x.Spell.Name,
            SpellDescription = x.Spell.Description,
            SpellGameType = x.Spell.GameType,
            LearnedAt = x.CharacterSpell.LearnedAt,
            Notes = x.CharacterSpell.Notes,
            IsPrepared = x.CharacterSpell.IsPrepared,
            SpellSlotLevel = x.CharacterSpell.SpellSlotLevel,
            CustomName = x.CharacterSpell.CustomName,
            School = x.Spell.School,
            Level = x.Spell.Level,
            CastingTime = x.Spell.CastingTime,
            Range = x.Spell.Range,
            Duration = x.Spell.Duration,
            IsRitual = x.Spell.IsRitual,
            RequiresConcentration = x.Spell.RequiresConcentration
        });
    }

    public async Task<bool> PrepareSpellAsync(int characterId, int spellId, bool isPrepared)
    {
        this.logger.LogInformation("Setting spell {SpellId} prepared status to {IsPrepared} for character {CharacterId}", 
            spellId, isPrepared, characterId);

        var characterSpell = await this.context.CharacterSpells
            .FirstOrDefaultAsync(cs => cs.CharacterId == characterId && cs.SpellId == spellId);

        if (characterSpell == null)
        {
            return false;
        }

        characterSpell.IsPrepared = isPrepared;
        await this.context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SetSpellSlotLevelAsync(int characterId, int spellId, int? slotLevel)
    {
        this.logger.LogInformation("Setting spell {SpellId} slot level to {SlotLevel} for character {CharacterId}", 
            spellId, slotLevel, characterId);

        var characterSpell = await this.context.CharacterSpells
            .FirstOrDefaultAsync(cs => cs.CharacterId == characterId && cs.SpellId == spellId);

        if (characterSpell == null)
        {
            return false;
        }

        characterSpell.SpellSlotLevel = slotLevel;
        await this.context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CanCharacterLearnSpellAsync(int characterId, int spellId)
    {
        // Récupérer le personnage
        var character = await this.context.CharactersDnd.FindAsync(characterId);
        if (character == null)
        {
            throw new InvalidOperationException($"Character with ID {characterId} not found");
        }

        // Récupérer le sort
        var spell = await this.context.SpellsDnd.FindAsync(spellId);
        if (spell == null)
        {
            throw new InvalidOperationException($"Spell with ID {spellId} not found");
        }

        // Validation GameType : 
        // - Sorts génériques : OK pour tous
        // - Sorts D&D : OK uniquement pour personnages D&D
        // - Sorts Skyrim : OK uniquement pour personnages Skyrim (futur)
        return spell.GameType switch
        {
            GameType.Generic => true, // Sorts génériques OK pour tous
            GameType.DnD => character.GameType == GameType.DnD, // Sorts D&D uniquement pour persos D&D
            GameType.Skyrim => character.GameType == GameType.Skyrim, // Futur
            _ => false
        };
    }

    public async Task<IEnumerable<SpellCompatibilityDto>> GetAvailableSpellsForCharacterAsync(int characterId, int userId)
    {
        this.logger.LogInformation("Getting available spells for character {CharacterId}", characterId);

        // Récupérer le personnage pour connaître son GameType
        var character = await this.context.CharactersDnd.FindAsync(characterId);
        if (character == null)
        {
            throw new InvalidOperationException($"Character with ID {characterId} not found");
        }

        // Récupérer tous les sorts disponibles (officiels + privés de l'utilisateur)
        var availableSpells = await this.context.SpellsDnd
            .Where(s => s.IsActive && (s.IsPublic || s.CreatedByUserId == userId))
            .ToListAsync();

        // Récupérer les sorts déjà appris par le personnage
        var learnedSpellIds = await this.context.CharacterSpells
            .Where(cs => cs.CharacterId == characterId)
            .Select(cs => cs.SpellId)
            .ToListAsync();

        return availableSpells.Select(spell => new SpellCompatibilityDto
        {
            SpellId = spell.Id,
            SpellName = spell.Name,
            SpellDescription = spell.Description,
            SpellGameType = spell.GameType,
            IsCompatible = this.IsSpellCompatibleWithCharacter(spell.GameType, character.GameType),
            IncompatibilityReason = this.GetIncompatibilityReason(spell.GameType, character.GameType),
            IsAlreadyLearned = learnedSpellIds.Contains(spell.Id),
            IsPublic = spell.IsPublic,
            Source = spell.Source.ToString(),
            School = spell.School,
            Level = spell.Level
        });
    }

    public async Task<IEnumerable<SpellCompatibilityDto>> SearchCompatibleSpellsAsync(int characterId, string searchText, int userId)
    {
        var availableSpells = await this.GetAvailableSpellsForCharacterAsync(characterId, userId);
        
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return availableSpells.Where(s => s.IsCompatible);
        }

        var searchTermLower = searchText.ToLower();
        return availableSpells.Where(s => s.IsCompatible && 
            (s.SpellName.ToLower().Contains(searchTermLower) || 
             s.SpellDescription.ToLower().Contains(searchTermLower) ||
             (s.School?.ToLower().Contains(searchTermLower) ?? false)));
    }

    private bool IsSpellCompatibleWithCharacter(GameType spellGameType, GameType characterGameType)
    {
        return spellGameType switch
        {
            GameType.Generic => true, // Sorts génériques OK pour tous
            GameType.DnD => characterGameType == GameType.DnD,
            GameType.Skyrim => characterGameType == GameType.Skyrim,
            _ => false
        };
    }

    private string? GetIncompatibilityReason(GameType spellGameType, GameType characterGameType)
    {
        if (this.IsSpellCompatibleWithCharacter(spellGameType, characterGameType))
        {
            return null;
        }

        return spellGameType switch
        {
            GameType.DnD => "Ce sort D&D n'est compatible qu'avec les personnages D&D",
            GameType.Skyrim => "Ce sort Skyrim n'est compatible qu'avec les personnages Skyrim",
            _ => "Système de jeu incompatible"
        };
    }
}