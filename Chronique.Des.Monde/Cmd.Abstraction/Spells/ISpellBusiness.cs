using Cdm.Common.Enums;

namespace Cmd.Abstraction.Spells;

/// <summary>
/// Interface abstraite pour les services de sorts, suivant le pattern ICharacterBusiness
/// </summary>
public interface ISpellBusiness
{
    Task<IEnumerable<ISpellView>> GetAllSpellsByUserId(int userId, GameType gameType);
    Task<ISpellView> GetSpellById(int spellId, int userId);
    Task<ISpellView> CreateSpell(SpellRequest spell, int userId);
    Task<ISpellView> UpdateSpell(SpellRequest spell, int spellId, int userId);
    Task DeleteSpell(int spellId, int userId);
    Task<IEnumerable<ISpellView>> SearchSpells(string searchText, int userId, GameType gameType);
}

/// <summary>
/// Interface pour les vues de sorts
/// </summary>
public interface ISpellView
{
    int Id { get; }
    string Name { get; }
    string Description { get; }
    string? ImageUrl { get; }
    GameType GameType { get; }
    bool IsPublic { get; }
    string Source { get; }
    List<string> Tags { get; }
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
}

/// <summary>
/// Modèle de requête générique pour les sorts
/// </summary>
public record SpellRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public GameType GameType { get; init; } = GameType.Generic;
    public List<string>? Tags { get; init; }
    public Dictionary<string, object>? SpecializedProperties { get; init; } // Pour les propriétés D&D
}