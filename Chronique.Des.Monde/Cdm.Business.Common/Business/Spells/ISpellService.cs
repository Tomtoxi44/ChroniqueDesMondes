using Cdm.Data.Models;
using Cdm.Common.Enums;

namespace Cdm.Business.Common.Business.Spells;

/// <summary>
/// Interface pour la gestion des sorts (générique)
/// Suit le pattern des autres services métier du projet
/// </summary>
public interface ISpellService
{
    /// <summary>
    /// Récupère tous les sorts officiels pour un type de jeu donné
    /// </summary>
    Task<List<ASpell>> GetOfficialSpellsAsync(GameType gameType);

    /// <summary>
    /// Récupère tous les sorts privés d'un utilisateur pour un type de jeu donné
    /// </summary>
    Task<List<ASpell>> GetUserPrivateSpellsAsync(int userId, GameType gameType);

    /// <summary>
    /// Récupère tous les sorts disponibles pour un utilisateur (officiels + privés)
    /// </summary>
    Task<List<ASpell>> GetAvailableSpellsAsync(int userId, GameType gameType);

    /// <summary>
    /// Récupère un sort par son ID
    /// </summary>
    Task<ASpell?> GetSpellByIdAsync(int spellId);

    /// <summary>
    /// Vérifie si un utilisateur peut modifier un sort
    /// </summary>
    Task<bool> CanUserModifySpellAsync(int userId, int spellId);

    /// <summary>
    /// Vérifie si un utilisateur peut voir un sort
    /// </summary>
    Task<bool> CanUserViewSpellAsync(int userId, int spellId);

    /// <summary>
    /// Crée un nouveau sort privé pour un utilisateur
    /// </summary>
    Task<ASpell> CreatePrivateSpellAsync(ASpell spell, int userId);

    /// <summary>
    /// Met à jour un sort existant
    /// </summary>
    Task<ASpell> UpdateSpellAsync(ASpell spell, int userId);

    /// <summary>
    /// Supprime (désactive) un sort
    /// </summary>
    Task DeleteSpellAsync(int spellId, int userId);

    /// <summary>
    /// Recherche des sorts par nom ou tags
    /// </summary>
    Task<List<ASpell>> SearchSpellsAsync(string searchText, GameType gameType, int userId);
}