using Cdm.Common.Enums;

namespace Cmd.Abstraction.Characters;

/// <summary>
/// Interface pour l'attribution et la gestion des sorts d'un personnage
/// </summary>
public interface ICharacterSpellService
{
    // Gestion des sorts du personnage
    Task<IEnumerable<CharacterSpellDto>> GetCharacterSpellsAsync(int characterId);
    Task<CharacterSpellDto> AddSpellToCharacterAsync(int characterId, int spellId, string? notes = null);
    Task RemoveSpellFromCharacterAsync(int characterId, int spellId);
    Task<CharacterSpellDto> UpdateCharacterSpellAsync(int characterId, int spellId, UpdateCharacterSpellRequest request);

    // Gestion des sorts préparés (D&D)
    Task<IEnumerable<CharacterSpellDto>> GetPreparedSpellsAsync(int characterId);
    Task<bool> PrepareSpellAsync(int characterId, int spellId, bool isPrepared);
    Task<bool> SetSpellSlotLevelAsync(int characterId, int spellId, int? slotLevel);

    // Validation de compatibilité
    Task<bool> CanCharacterLearnSpellAsync(int characterId, int spellId);
    Task<IEnumerable<SpellCompatibilityDto>> GetAvailableSpellsForCharacterAsync(int characterId, int userId);

    // Recherche de sorts compatibles
    Task<IEnumerable<SpellCompatibilityDto>> SearchCompatibleSpellsAsync(int characterId, string searchText, int userId);
}

/// <summary>
/// DTO pour l'affichage des sorts d'un personnage
/// </summary>
public record CharacterSpellDto
{
    public int Id { get; init; }
    public int CharacterId { get; init; }
    public int SpellId { get; init; }
    public string SpellName { get; init; } = string.Empty;
    public string SpellDescription { get; init; } = string.Empty;
    public GameType SpellGameType { get; init; }
    public DateTime LearnedAt { get; init; }
    public string? Notes { get; init; }
    public bool IsPrepared { get; init; }
    public int? SpellSlotLevel { get; init; }
    public string? CustomName { get; init; }
    
    // Propriétés D&D spécifiques (si applicable)
    public string? School { get; init; }
    public int? Level { get; init; }
    public string? CastingTime { get; init; }
    public string? Range { get; init; }
    public string? Duration { get; init; }
    public bool IsRitual { get; init; }
    public bool RequiresConcentration { get; init; }
}

/// <summary>
/// DTO pour l'affichage de la compatibilité d'un sort avec un personnage
/// </summary>
public record SpellCompatibilityDto
{
    public int SpellId { get; init; }
    public string SpellName { get; init; } = string.Empty;
    public string SpellDescription { get; init; } = string.Empty;
    public GameType SpellGameType { get; init; }
    public bool IsCompatible { get; init; }
    public string? IncompatibilityReason { get; init; }
    public bool IsAlreadyLearned { get; init; }
    public bool IsPublic { get; init; }
    public string Source { get; init; } = string.Empty; // "Official" ou "Private"
    
    // Propriétés D&D (si applicable)
    public string? School { get; init; }
    public int? Level { get; init; }
}

/// <summary>
/// Modèle de requête pour mettre à jour un sort de personnage
/// </summary>
public record UpdateCharacterSpellRequest
{
    public string? Notes { get; init; }
    public bool? IsPrepared { get; init; }
    public int? SpellSlotLevel { get; init; }
    public string? CustomName { get; init; }
}