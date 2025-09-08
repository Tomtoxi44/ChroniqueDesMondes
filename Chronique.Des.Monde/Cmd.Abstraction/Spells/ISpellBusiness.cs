using Cdm.Common.Enums;

namespace Cmd.Abstraction.Spells;

/// <summary>
/// Interface abstraite pour les services de sorts, suivant le pattern ICharacterBusiness
/// </summary>
public interface ISpellBusiness
{
    // === MÉTHODES EXISTANTES ===
    Task<IEnumerable<ISpellView>> GetAllSpellsByUserId(int userId, GameType gameType);
    Task<ISpellView> GetSpellById(int spellId, int userId);
    Task<ISpellView> CreateSpell(SpellRequest spell, int userId);
    Task<ISpellView> UpdateSpell(SpellRequest spell, int spellId, int userId);
    Task DeleteSpell(int spellId, int userId);
    Task<IEnumerable<ISpellView>> SearchSpells(string searchText, int userId, GameType gameType);

    // === NOUVELLES MÉTHODES PRIORITÉ 2 (selon documentation) ===

    /// <summary>
    /// Récupère les sorts officiels uniquement (créés par l'administration - CreatedByUserId = 0)
    /// Visible par tous les utilisateurs sans authentification
    /// </summary>
    Task<IEnumerable<ISpellView>> GetOfficialSpellsAsync(GameType gameType);

    /// <summary>
    /// Récupère les sorts privés d'un utilisateur spécifique uniquement
    /// Visible uniquement par le créateur (CreatedByUserId = userId)
    /// </summary>
    Task<IEnumerable<ISpellView>> GetUserPrivateSpellsAsync(int userId, GameType gameType);

    /// <summary>
    /// Vérifie si un utilisateur peut modifier un sort
    /// Règles : sorts officiels (CreatedByUserId = 0) NON modifiables, sorts privés modifiables par leur créateur
    /// </summary>
    Task<bool> CanUserModifySpellAsync(int userId, int spellId);

    /// <summary>
    /// Récupère les sorts par école de magie (D&D uniquement)
    /// Ex: "Évocation", "Enchantement", "Abjuration"
    /// </summary>
    Task<IEnumerable<ISpellView>> GetSpellsBySchoolAsync(string school, int userId, GameType gameType);

    /// <summary>
    /// Récupère les sorts par niveau (D&D uniquement)
    /// Niveau 0 = Cantrips, Niveaux 1-9 = Sorts avec emplacements
    /// </summary>
    Task<IEnumerable<ISpellView>> GetSpellsByLevelAsync(int level, int userId, GameType gameType);
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