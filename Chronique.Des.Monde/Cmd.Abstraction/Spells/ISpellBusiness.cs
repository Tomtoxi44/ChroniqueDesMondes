using Cdm.Business.Common.Models.Spells;
using Cdm.Common.Enums;

namespace Cmd.Abstraction.Spells;

/// <summary>
/// Interface abstraite pour les services de sorts, suivant le pattern ICharacterBusiness
/// </summary>
public interface ISpellBusiness
{
    // === MÉTHODES EXISTANTES ===
    Task<IEnumerable<SpellView>> GetAllSpellsByUserId(int userId, GameType gameType);
    Task<SpellView> GetSpellById(int id, int userId);
    Task<SpellView> CreateSpell(SpellRequest request, int userId);
    Task<SpellView> UpdateSpell(SpellRequest request, int id, int userId);
    Task DeleteSpell(int id, int userId);
    Task<IEnumerable<SpellView>> SearchSpells(string searchText, int userId, GameType gameType);

    // === NOUVELLES MÉTHODES SELON DOCUMENTATION ===

    /// <summary>
    /// Récupère les sorts officiels uniquement (créés par l'administration)
    /// Visible par tous les utilisateurs
    /// </summary>
    Task<IEnumerable<SpellView>> GetOfficialSpellsAsync(GameType gameType);

    /// <summary>
    /// Récupère les sorts privés d'un utilisateur spécifique uniquement
    /// Visible uniquement par le créateur
    /// </summary>
    Task<IEnumerable<SpellView>> GetUserPrivateSpellsAsync(int userId, GameType gameType);

    /// <summary>
    /// Vérifie si un utilisateur peut modifier un sort
    /// (sorts officiels non modifiables, sorts privés modifiables par leur créateur)
    /// </summary>
    Task<bool> CanUserModifySpellAsync(int userId, int spellId);

    /// <summary>
    /// Récupère les sorts par école de magie (D&D uniquement)
    /// </summary>
    Task<IEnumerable<SpellView>> GetSpellsBySchoolAsync(string school, int userId, GameType gameType);

    /// <summary>
    /// Récupère les sorts par niveau (D&D uniquement)
    /// </summary>
    Task<IEnumerable<SpellView>> GetSpellsByLevelAsync(int level, int userId, GameType gameType);
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