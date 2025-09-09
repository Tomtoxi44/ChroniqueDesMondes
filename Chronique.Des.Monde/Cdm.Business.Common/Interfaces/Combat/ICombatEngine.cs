using Cdm.Data.Common.Models.Combat;
using Cdm.Business.Common.Models.Combat;

namespace Cdm.Business.Common.Interfaces.Combat;

/// <summary>
/// Interface pour le moteur de combat principal
/// Gère tous les aspects du combat (initiative, actions, règles)
/// </summary>
public interface ICombatEngine
{
    /// <summary>
    /// Démarre un nouveau combat
    /// </summary>
    Task<CombatStateDto> StartCombatAsync(StartCombatCommand command);

    /// <summary>
    /// Obtient l'état actuel d'un combat
    /// </summary>
    Task<CombatStateDto?> GetCombatStateAsync(int combatId);

    /// <summary>
    /// Ajoute un participant à un combat en cours (invitation dynamique)
    /// </summary>
    Task<CombatStateDto> AddParticipantAsync(int combatId, CombatParticipantDto participant);

    /// <summary>
    /// Lance l'initiative pour un participant
    /// </summary>
    Task<CombatStateDto> RollInitiativeAsync(int combatId, int participantId, int initiativeRoll);

    /// <summary>
    /// Démarre la phase de combat (après que toutes les initiatives soient lancées)
    /// </summary>
    Task<CombatStateDto> StartCombatPhaseAsync(int combatId);

    /// <summary>
    /// Exécute une action de combat
    /// </summary>
    Task<CombatActionResult> ExecuteActionAsync(ExecuteCombatActionCommand command);

    /// <summary>
    /// Passe au tour suivant
    /// </summary>
    Task<CombatStateDto> AdvanceToNextTurnAsync(int combatId);

    /// <summary>
    /// Termine un combat
    /// </summary>
    Task<CombatStateDto> EndCombatAsync(int combatId, string? winner = null);

    /// <summary>
    /// Met en pause ou reprend un combat
    /// </summary>
    Task<CombatStateDto> PauseCombatAsync(int combatId, bool isPaused);

    /// <summary>
    /// Applique un effet de statut à un participant
    /// </summary>
    Task<StatusEffectResult> ApplyStatusEffectAsync(int participantId, StatusEffectDto effect);

    /// <summary>
    /// Supprime un effet de statut
    /// </summary>
    Task<bool> RemoveStatusEffectAsync(int effectId);

    /// <summary>
    /// Calcule les dégâts pour une attaque
    /// </summary>
    Task<CombatActionResult> CalculateDamageAsync(int attackerId, int? targetId, int equipmentId, List<int>? diceRolls = null);

    /// <summary>
    /// Lance un sort en combat
    /// </summary>
    Task<CombatActionResult> CastSpellAsync(int casterId, int spellId, int spellLevel, int? targetId = null, List<int>? diceRolls = null);

    /// <summary>
    /// Valide si un participant peut effectuer une action
    /// </summary>
    Task<(bool CanAct, string? Reason)> CanParticipantActAsync(int combatId, int participantId);

    /// <summary>
    /// Obtient les actions disponibles pour un participant
    /// </summary>
    Task<List<string>> GetAvailableActionsAsync(int participantId);
}

/// <summary>
/// Interface pour les calculs spécifiques à D&D 5e
/// </summary>
public interface IDndCombatCalculator
{
    /// <summary>
    /// Calcule le modificateur d'une caractéristique D&D
    /// </summary>
    int CalculateAbilityModifier(int abilityScore);

    /// <summary>
    /// Calcule le bonus de maîtrise selon le niveau
    /// </summary>
    int CalculateProficiencyBonus(int characterLevel);

    /// <summary>
    /// Calcule la classe d'armure totale
    /// </summary>
    Task<int> CalculateArmorClassAsync(int characterId);

    /// <summary>
    /// Calcule le bonus d'attaque total
    /// </summary>
    Task<int> CalculateAttackBonusAsync(int characterId, int equipmentId);

    /// <summary>
    /// Calcule les dégâts d'une arme
    /// </summary>
    Task<(string DiceFormula, int Modifier)> CalculateWeaponDamageAsync(int characterId, int weaponId, bool isCritical = false);

    /// <summary>
    /// Calcule le DD de sauvegarde d'un sort
    /// </summary>
    Task<int> CalculateSpellSaveDCAsync(int characterId, int spellId);

    /// <summary>
    /// Calcule le bonus d'attaque magique
    /// </summary>
    Task<int> CalculateSpellAttackBonusAsync(int characterId);

    /// <summary>
    /// Vérifie si un personnage peut lancer un sort
    /// </summary>
    Task<(bool CanCast, string? Reason)> CanCastSpellAsync(int characterId, int spellId, int spellLevel);

    /// <summary>
    /// Consomme un emplacement de sort
    /// </summary>
    Task ConsumeSpellSlotAsync(int characterId, int spellLevel);
}

/// <summary>
/// Interface pour la gestion de l'initiative
/// </summary>
public interface IInitiativeManager
{
    /// <summary>
    /// Calcule l'ordre d'initiative pour tous les participants
    /// </summary>
    List<InitiativeOrderDto> CalculateTurnOrder(List<CombatParticipant> participants);

    /// <summary>
    /// Obtient le participant suivant dans l'ordre
    /// </summary>
    CombatParticipant? GetNextParticipant(List<CombatParticipant> participants, int currentIndex);

    /// <summary>
    /// Gère les égalités d'initiative
    /// </summary>
    List<CombatParticipant> ResolveTies(List<CombatParticipant> participants);

    /// <summary>
    /// Insère un nouveau participant dans l'ordre existant
    /// </summary>
    int InsertParticipantInOrder(List<CombatParticipant> participants, CombatParticipant newParticipant);
}

/// <summary>
/// Interface pour la génération de dés
/// </summary>
public interface IDiceRoller
{
    /// <summary>
    /// Lance un dé simple (1d20, 1d6, etc.)
    /// </summary>
    int RollDie(int sides);

    /// <summary>
    /// Lance plusieurs dés (2d6, 3d8, etc.)
    /// </summary>
    List<int> RollDice(int count, int sides);

    /// <summary>
    /// Analyse et lance une formule de dé (1d8+3, 2d6+2, etc.)
    /// </summary>
    DiceRollResult RollFormula(string formula, int modifier = 0);

    /// <summary>
    /// Lance avec avantage (2d20 prendre le plus haut)
    /// </summary>
    DiceRollResult RollWithAdvantage(int sides = 20);

    /// <summary>
    /// Lance avec désavantage (2d20 prendre le plus bas)
    /// </summary>
    DiceRollResult RollWithDisadvantage(int sides = 20);

    /// <summary>
    /// Lance des dégâts avec possibilité de critique
    /// </summary>
    DiceRollResult RollDamage(string diceFormula, int modifier, bool isCritical = false);

    /// <summary>
    /// Lance un jet d'attaque avec modificateurs D&D 5e
    /// </summary>
    DiceRollResult RollAttack(int attackBonus, string advantageType = "normal");

    /// <summary>
    /// Lance un jet de sauvegarde avec modificateurs D&D 5e
    /// </summary>
    DiceRollResult RollSavingThrow(int saveModifier, string advantageType = "normal");

    /// <summary>
    /// Lance l'initiative avec modificateur de Dextérité
    /// </summary>
    DiceRollResult RollInitiative(int dexModifier);
}