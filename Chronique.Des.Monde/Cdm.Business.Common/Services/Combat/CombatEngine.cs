using Cdm.Business.Common.Interfaces.Combat;
using Cdm.Business.Common.Models.Combat;
using Cdm.Data.Common.Models.Combat;
using Cdm.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using Cdm.Common.Enums;

namespace Cdm.Business.Common.Services.Combat;

/// <summary>
/// Moteur de combat principal gérant tous les types de jeux
/// Utilise les calculateurs spécialisés selon le GameType
/// </summary>
public class CombatEngine : ICombatEngine
{
    private readonly AppDbContext _context;
    private readonly IInitiativeManager _initiativeManager;
    private readonly IDiceRoller _diceRoller;
    private readonly IDndCombatCalculator? _dndCalculator;
    private readonly ILogger<CombatEngine> _logger;

    public CombatEngine(
        AppDbContext context,
        IInitiativeManager initiativeManager,
        IDiceRoller diceRoller,
        ILogger<CombatEngine> logger,
        IDndCombatCalculator? dndCalculator = null)
    {
        _context = context;
        _initiativeManager = initiativeManager;
        _diceRoller = diceRoller;
        _dndCalculator = dndCalculator;
        _logger = logger;
    }

    /// <summary>
    /// Démarre un nouveau combat
    /// </summary>
    public async Task<CombatStateDto> StartCombatAsync(StartCombatCommand command)
    {
        _logger.LogInformation("🎯 Démarrage combat: {Name} ({GameType})", command.Name, command.GameType);

        // Créer la session de combat
        var combat = new CombatSession
        {
            SessionId = command.SessionId,
            ChapterId = command.ChapterId,
            Name = command.Name,
            GameType = command.GameType,
            Status = "initiative",
            CurrentRound = 1,
            CurrentTurnIndex = 0,
            StartedAt = DateTime.UtcNow,
            Environment = JsonSerializer.Serialize(command.Environment),
            Settings = JsonSerializer.Serialize(command.Settings)
        };

        _context.CombatSessions.Add(combat);
        await _context.SaveChangesAsync();

        // Ajouter les participants
        foreach (var participantDto in command.Participants)
        {
            var participant = await CreateParticipantFromDtoAsync(combat.Id, participantDto);
            _context.CombatParticipants.Add(participant);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("⚔️ Combat {CombatId} créé avec {Count} participants", 
            combat.Id, command.Participants.Count);

        return await GetCombatStateAsync(combat.Id) ?? throw new InvalidOperationException("Combat créé mais non trouvé");
    }

    /// <summary>
    /// Obtient l'état actuel d'un combat
    /// </summary>
    public async Task<CombatStateDto?> GetCombatStateAsync(int combatId)
    {
        var combat = await _context.CombatSessions
            .Include(c => c.Participants)
            .Include(c => c.Actions)
            .FirstOrDefaultAsync(c => c.Id == combatId);

        if (combat == null) return null;

        var turnOrder = _initiativeManager.CalculateTurnOrder(combat.Participants.ToList());
        var currentParticipant = combat.GetCurrentParticipant();

        var state = new CombatStateDto
        {
            Id = combat.Id,
            SessionId = combat.SessionId,
            Name = combat.Name,
            Status = combat.Status,
            CurrentRound = combat.CurrentRound,
            CurrentTurnIndex = combat.CurrentTurnIndex,
            Participants = combat.Participants.Select(MapToParticipantState).ToList(),
            TurnOrder = turnOrder,
            CurrentParticipant = currentParticipant != null ? MapToParticipantState(currentParticipant) : null,
            Environment = ParseEnvironment(combat.Environment),
            ActionLog = GetRecentActionLog(combat.Actions.ToList()),
            StartedAt = combat.StartedAt,
            TurnTimeLimit = combat.TurnTimeLimit,
            CurrentTurnStarted = combat.CurrentTurnStarted,
            TimeRemaining = CalculateTimeRemaining(combat)
        };

        return state;
    }

    /// <summary>
    /// Ajoute un participant à un combat en cours (invitation dynamique)
    /// </summary>
    public async Task<CombatStateDto> AddParticipantAsync(int combatId, CombatParticipantDto participantDto)
    {
        var combat = await _context.CombatSessions
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == combatId);

        if (combat == null)
            throw new InvalidOperationException($"Combat {combatId} non trouvé");

        if (combat.Status == "ended")
            throw new InvalidOperationException("Impossible d'ajouter un participant à un combat terminé");

        _logger.LogInformation("🆕 Ajout participant dynamique au combat {CombatId}", combatId);

        var participant = await CreateParticipantFromDtoAsync(combatId, participantDto);
        _context.CombatParticipants.Add(participant);
        await _context.SaveChangesAsync();

        return await GetCombatStateAsync(combatId) ?? throw new InvalidOperationException("Combat non trouvé après ajout participant");
    }

    /// <summary>
    /// Lance l'initiative pour un participant
    /// </summary>
    public async Task<CombatStateDto> RollInitiativeAsync(int combatId, int participantId, int initiativeRoll)
    {
        var participant = await _context.CombatParticipants
            .FirstOrDefaultAsync(p => p.Id == participantId && p.CombatId == combatId);

        if (participant == null)
            throw new InvalidOperationException($"Participant {participantId} non trouvé dans le combat {combatId}");

        participant.Initiative = initiativeRoll;
        
        // Calculer le tiebreaker (modificateur de Dextérité pour D&D)
        if (participant.CharacterId.HasValue && _dndCalculator != null)
        {
            // Pour D&D, utiliser le modificateur de Dextérité comme tiebreaker
            // Simplifié - en production, récupérer depuis le personnage
            participant.InitiativeTiebreaker = _diceRoller.RollDie(6) - 3; // Simulé
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("🎲 Initiative {Initiative} lancée pour {ParticipantName}", 
            initiativeRoll, participant.Name);

        return await GetCombatStateAsync(combatId) ?? throw new InvalidOperationException("Combat non trouvé");
    }

    /// <summary>
    /// Démarre la phase de combat (après que toutes les initiatives soient lancées)
    /// </summary>
    public async Task<CombatStateDto> StartCombatPhaseAsync(int combatId)
    {
        var combat = await _context.CombatSessions
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == combatId);

        if (combat == null)
            throw new InvalidOperationException($"Combat {combatId} non trouvé");

        if (combat.Status != "initiative")
            throw new InvalidOperationException("Combat pas en phase d'initiative");

        if (!combat.AllInitiativesRolled())
            throw new InvalidOperationException("Toutes les initiatives ne sont pas encore lancées");

        combat.Status = "active";
        combat.CurrentTurnStarted = DateTime.UtcNow;

        // Calculer l'ordre d'initiative final
        var turnOrder = _initiativeManager.CalculateTurnOrder(combat.Participants.ToList());
        combat.TurnOrder = JsonSerializer.Serialize(turnOrder);

        await _context.SaveChangesAsync();

        _logger.LogInformation("⚔️ Combat {CombatId} démarré ! Premier tour: {FirstParticipant}", 
            combatId, turnOrder.FirstOrDefault()?.Name);

        return await GetCombatStateAsync(combatId) ?? throw new InvalidOperationException("Combat non trouvé");
    }

    /// <summary>
    /// Exécute une action de combat
    /// </summary>
    public async Task<CombatActionResult> ExecuteActionAsync(ExecuteCombatActionCommand command)
    {
        var startTime = DateTime.UtcNow;

        var combat = await _context.CombatSessions
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == command.CombatId);

        if (combat == null)
            return new CombatActionResult { Success = false, ErrorMessage = "Combat non trouvé" };

        var actor = combat.Participants.FirstOrDefault(p => p.Id == command.ActorId);
        if (actor == null)
            return new CombatActionResult { Success = false, ErrorMessage = "Acteur non trouvé" };

        // Vérifier que c'est le tour de l'acteur
        var currentParticipant = combat.GetCurrentParticipant();
        if (currentParticipant?.Id != command.ActorId)
            return new CombatActionResult { Success = false, ErrorMessage = "Ce n'est pas votre tour" };

        _logger.LogInformation("⚡ Exécution action {ActionType} par {ActorName}", 
            command.ActionType, actor.Name);

        // Exécuter l'action selon le type
        var result = command.ActionType.ToLower() switch
        {
            "weapon_attack" => await ExecuteWeaponAttackAsync(actor, command),
            "spell_attack" => await ExecuteSpellAttackAsync(actor, command),
            "spell_save" => await ExecuteSpellSaveAsync(actor, command),
            "attack" => await ExecuteWeaponAttackAsync(actor, command), // Alias pour compatibilité
            "spell" => await ExecuteSpellCastAsync(actor, command), // Alias pour compatibilité
            "move" => await ExecuteMoveAsync(actor, command),
            "dodge" => ExecuteDodge(actor),
            "help" => ExecuteHelp(actor, command),
            "end_turn" => ExecuteEndTurn(actor),
            _ => new CombatActionResult { Success = false, ErrorMessage = "Type d'action non reconnu" }
        };

        // Enregistrer l'action
        if (result.Success)
        {
            var combatAction = new CombatAction
            {
                CombatId = command.CombatId,
                ActorId = command.ActorId,
                TargetId = command.TargetId,
                Round = combat.CurrentRound,
                ActionType = command.ActionType,
                ActionName = command.ActionName,
                Description = result.ActionDescription,
                EquipmentId = command.EquipmentId,
                SpellId = command.SpellId,
                SpellLevel = command.SpellLevel,
                DiceRolls = JsonSerializer.Serialize(result.DiceRolls),
                DamageDealt = result.DamageDealt,
                DamageType = result.DamageType,
                HealingDone = result.HealingDone,
                IsSuccess = result.Success,
                IsCritical = result.IsCritical,
                ExecutedAt = startTime,
                ExecutionTime = DateTime.UtcNow - startTime
            };

            _context.CombatActions.Add(combatAction);
            actor.TurnsPlayed++;
            actor.LastTurnAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Mettre à jour les participants dans le résultat
            result.UpdatedActor = MapToParticipantState(actor);
            if (command.TargetId.HasValue)
            {
                var target = combat.Participants.FirstOrDefault(p => p.Id == command.TargetId.Value);
                if (target != null)
                {
                    result.UpdatedTarget = MapToParticipantState(target);
                    result.TargetName = target.Name;
                }
            }
        }

        result.ExecutionTime = DateTime.UtcNow - startTime;
        result.ActorName = actor.Name;

        _logger.LogInformation("✅ Action {ActionType} exécutée en {ExecutionTime}ms: {Success}", 
            command.ActionType, result.ExecutionTime.TotalMilliseconds, result.Success);

        return result;
    }

    /// <summary>
    /// Passe au tour suivant
    /// </summary>
    public async Task<CombatStateDto> AdvanceToNextTurnAsync(int combatId)
    {
        var combat = await _context.CombatSessions
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == combatId);

        if (combat == null)
            throw new InvalidOperationException($"Combat {combatId} non trouvé");

        if (combat.Status != "active")
            throw new InvalidOperationException("Combat pas actif");

        var activeParticipants = combat.Participants
            .Where(p => p.IsActive && p.CurrentHitPoints > 0)
            .OrderByDescending(p => p.Initiative)
            .ThenByDescending(p => p.InitiativeTiebreaker)
            .ToList();

        if (!activeParticipants.Any())
        {
            combat.Status = "ended";
            await _context.SaveChangesAsync();
            throw new InvalidOperationException("Plus de participants actifs");
        }

        // Passer au participant suivant
        combat.CurrentTurnIndex = (combat.CurrentTurnIndex + 1) % activeParticipants.Count;
        
        // Si on revient au premier participant, c'est un nouveau round
        if (combat.CurrentTurnIndex == 0)
        {
            combat.CurrentRound++;
            _logger.LogInformation("🔄 Nouveau round {Round} du combat {CombatId}", 
                combat.CurrentRound, combatId);
        }

        combat.CurrentTurnStarted = DateTime.UtcNow;

        // Vérifier si le combat est terminé
        if (combat.IsCombatOver())
        {
            combat.Status = "ended";
            combat.EndedAt = DateTime.UtcNow;
            
            var winner = DetermineWinner(combat.Participants.ToList());
            _logger.LogInformation("🏆 Combat {CombatId} terminé ! Vainqueur: {Winner}", 
                combatId, winner);
        }

        await _context.SaveChangesAsync();

        return await GetCombatStateAsync(combatId) ?? throw new InvalidOperationException("Combat non trouvé");
    }

    /// <summary>
    /// Termine un combat
    /// </summary>
    public async Task<CombatStateDto> EndCombatAsync(int combatId, string? winner = null)
    {
        var combat = await _context.CombatSessions
            .FirstOrDefaultAsync(c => c.Id == combatId);

        if (combat == null)
            throw new InvalidOperationException($"Combat {combatId} non trouvé");

        combat.Status = "ended";
        combat.EndedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("🏁 Combat {CombatId} terminé manuellement. Vainqueur: {Winner}", 
            combatId, winner ?? "Non spécifié");

        return await GetCombatStateAsync(combatId) ?? throw new InvalidOperationException("Combat non trouvé");
    }

    /// <summary>
    /// Met en pause ou reprend un combat
    /// </summary>
    public async Task<CombatStateDto> PauseCombatAsync(int combatId, bool isPaused)
    {
        var combat = await _context.CombatSessions
            .FirstOrDefaultAsync(c => c.Id == combatId);

        if (combat == null)
            throw new InvalidOperationException($"Combat {combatId} non trouvé");

        combat.Status = isPaused ? "paused" : "active";
        await _context.SaveChangesAsync();

        _logger.LogInformation("⏸️ Combat {CombatId} {Status}", combatId, isPaused ? "mis en pause" : "repris");

        return await GetCombatStateAsync(combatId) ?? throw new InvalidOperationException("Combat non trouvé");
    }

    /// <summary>
    /// Applique un effet de statut à un participant
    /// </summary>
    public async Task<StatusEffectResult> ApplyStatusEffectAsync(int participantId, StatusEffectDto effect)
    {
        var participant = await _context.CombatParticipants
            .FirstOrDefaultAsync(p => p.Id == participantId);

        if (participant == null)
            throw new InvalidOperationException($"Participant {participantId} non trouvé");

        var statusEffect = new CombatStatusEffect
        {
            ParticipantId = participantId,
            Name = effect.Name,
            EffectType = effect.EffectType,
            Description = effect.Description,
            Duration = effect.Duration,
            InitialDuration = effect.Duration,
            Intensity = effect.Intensity,
            IsActive = true,
            Icon = effect.Icon,
            CanBeDispelled = effect.CanBeDispelled,
            AppliedAtRound = participant.Combat?.CurrentRound ?? 1
        };

        _context.CombatStatusEffects.Add(statusEffect);
        await _context.SaveChangesAsync();

        _logger.LogInformation("✨ Effet {EffectName} appliqué à {ParticipantName}", 
            effect.Name, participant.Name);

        return new StatusEffectResult
        {
            EffectName = effect.Name,
            TargetName = participant.Name,
            Applied = true,
            Duration = effect.Duration
        };
    }

    /// <summary>
    /// Supprime un effet de statut
    /// </summary>
    public async Task<bool> RemoveStatusEffectAsync(int effectId)
    {
        var effect = await _context.CombatStatusEffects
            .FirstOrDefaultAsync(e => e.Id == effectId);

        if (effect == null) return false;

        effect.Remove();
        await _context.SaveChangesAsync();

        _logger.LogInformation("🧹 Effet {EffectName} supprimé", effect.Name);

        return true;
    }

    /// <summary>
    /// Calcule les dégâts pour une attaque
    /// </summary>
    public async Task<CombatActionResult> CalculateDamageAsync(int attackerId, int? targetId, int equipmentId, List<int>? diceRolls = null)
    {
        // Implementation sera complétée selon le GameType
        return new CombatActionResult { Success = false, ErrorMessage = "Non implémenté" };
    }

    /// <summary>
    /// Lance un sort en combat
    /// </summary>
    public async Task<CombatActionResult> CastSpellAsync(int casterId, int spellId, int spellLevel, int? targetId = null, List<int>? diceRolls = null)
    {
        // Implementation sera complétée selon le GameType
        return new CombatActionResult { Success = false, ErrorMessage = "Non implémenté" };
    }

    /// <summary>
    /// Valide si un participant peut effectuer une action
    /// </summary>
    public async Task<(bool CanAct, string? Reason)> CanParticipantActAsync(int combatId, int participantId)
    {
        var combat = await _context.CombatSessions
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == combatId);

        if (combat == null)
            return (false, "Combat non trouvé");

        var participant = combat.Participants.FirstOrDefault(p => p.Id == participantId);
        if (participant == null)
            return (false, "Participant non trouvé");

        if (!participant.IsActive)
            return (false, "Participant inactif");

        if (participant.CurrentHitPoints <= 0)
            return (false, "Participant inconscient");

        var currentParticipant = combat.GetCurrentParticipant();
        if (currentParticipant?.Id != participantId)
            return (false, "Ce n'est pas votre tour");

        return (true, null);
    }

    /// <summary>
    /// Obtient les actions disponibles pour un participant
    /// </summary>
    public async Task<List<string>> GetAvailableActionsAsync(int participantId)
    {
        var participant = await _context.CombatParticipants
            .FirstOrDefaultAsync(p => p.Id == participantId);

        if (participant == null) return new List<string>();

        var baseActions = new List<string> { "attack", "move", "dodge", "help", "end_turn" };

        // Ajouter les actions de sort si le participant peut lancer des sorts
        if (participant.CharacterId.HasValue)
        {
            baseActions.Add("spell");
        }

        return baseActions;
    }

    #region Méthodes privées d'exécution d'actions

    /// <summary>
    /// Exécute une attaque d'arme avec calculs D&D 5e complets
    /// </summary>
    private async Task<CombatActionResult> ExecuteWeaponAttackAsync(CombatParticipant actor, ExecuteCombatActionCommand command)
    {
        if (!command.EquipmentId.HasValue)
        {
            return new CombatActionResult 
            { 
                Success = false, 
                ErrorMessage = "ID d'équipement requis pour l'attaque d'arme" 
            };
        }

        if (!command.TargetId.HasValue)
        {
            return new CombatActionResult 
            { 
                Success = false, 
                ErrorMessage = "Cible requise pour l'attaque d'arme" 
            };
        }

        // Récupérer la cible
        var target = await _context.CombatParticipants
            .FirstOrDefaultAsync(p => p.Id == command.TargetId.Value);

        if (target == null)
        {
            return new CombatActionResult 
            { 
                Success = false, 
                ErrorMessage = "Cible non trouvée" 
            };
        }

        // Pour D&D, utiliser le calculateur spécialisé
        if (actor.Combat?.GameType == GameType.DnD && _dndCalculator != null && actor.CharacterId.HasValue)
        {
            return await ExecuteDndWeaponAttackAsync(actor, target, command.EquipmentId.Value, command.DiceRolls);
        }

        // Attaque générique pour autres systèmes
        return await ExecuteGenericWeaponAttackAsync(actor, target, command.EquipmentId.Value, command.DiceRolls);
    }

    /// <summary>
    /// Exécute une attaque d'arme D&D 5e avec tous les calculs automatiques
    /// </summary>
    private async Task<CombatActionResult> ExecuteDndWeaponAttackAsync(
        CombatParticipant attacker, 
        CombatParticipant target, 
        int weaponId, 
        List<int>? forcedRolls = null)
    {
        try
        {
            // 1. Récupérer l'arme depuis tes équipements D&D
            var weapon = await GetDndEquipmentAsync(weaponId);
            if (weapon == null || !weapon.IsWeapon)
            {
                return new CombatActionResult 
                { 
                    Success = false, 
                    ErrorMessage = "Arme D&D non trouvée ou invalide" 
                };
            }

            // 2. Calculer le bonus d'attaque avec le calculateur D&D
            var attackBonus = await _dndCalculator!.CalculateAttackBonusAsync(
                attacker.CharacterId!.Value, weaponId);

            // 3. Déterminer l'avantage/désavantage (simplifié pour l'instant)
            var advantageType = DetermineAdvantageType(attacker, target, weapon);

            // 4. Lancer l'attaque
            var attackRoll = forcedRolls?.Count > 0 ? 
                CreateForcedAttackRoll(forcedRolls[0], attackBonus) :
                _diceRoller.RollAttack(attackBonus, advantageType);

            // 5. Vérifier si l'attaque touche
            var hit = attackRoll.Total >= target.ArmorClass;
            var naturalRoll = advantageType == "advantage" ? attackRoll.Rolls.Max() : 
                             advantageType == "disadvantage" ? attackRoll.Rolls.Min() : 
                             attackRoll.Rolls[0];
            var isCritical = naturalRoll == 20;
            var isCriticalMiss = naturalRoll == 1;

            var result = new CombatActionResult
            {
                Success = true,
                ActionDescription = "",
                ActorName = attacker.Name,
                TargetName = target.Name,
                DiceRolls = [attackRoll],
                IsCritical = isCritical
            };

            // 6. Si l'attaque touche, calculer les dégâts
            if (hit && !isCriticalMiss)
            {
                var (damageFormula, damageModifier) = await _dndCalculator.CalculateWeaponDamageAsync(
                    attacker.CharacterId.Value, weaponId, isCritical);

                var damageRoll = forcedRolls?.Count > 1 ? 
                    CreateForcedDamageRoll(forcedRolls.Skip(1).ToList(), damageModifier, damageFormula) :
                    _diceRoller.RollDamage(damageFormula, damageModifier, isCritical);

                result.DiceRolls.Add(damageRoll);
                result.DamageDealt = damageRoll.Total;
                result.DamageType = weapon.DamageType;

                // Appliquer les dégâts à la cible
                target.TakeDamage(damageRoll.Total);
                result.TargetDefeated = !target.IsAlive;

                // Description détaillée du succès
                result.ActionDescription = GenerateWeaponAttackDescription(
                    attacker, target, weapon, attackRoll, damageRoll, isCritical);
            }
            else
            {
                // Description de l'échec
                result.ActionDescription = isCriticalMiss ?
                    $"💀 {attacker.Name} rate complètement son attaque avec {weapon.Name} ! (Échec critique)" :
                    $"❌ {attacker.Name} attaque avec {weapon.Name} mais rate {target.Name} (Jet: {attackRoll.Total} vs CA {target.ArmorClass})";
            }

            _logger.LogInformation("⚔️ Attaque D&D: {Attacker} -> {Target}, Jet: {Attack}, Touche: {Hit}, Dégâts: {Damage}", 
                attacker.Name, target.Name, attackRoll.Total, hit, result.DamageDealt ?? 0);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'attaque D&D de {Attacker}", attacker.Name);
            return new CombatActionResult 
            { 
                Success = false, 
                ErrorMessage = $"Erreur lors de l'attaque: {ex.Message}" 
            };
        }
    }

    /// <summary>
    /// Exécute une attaque générique pour les systèmes non-D&D
    /// </summary>
    private async Task<CombatActionResult> ExecuteGenericWeaponAttackAsync(
        CombatParticipant attacker, 
        CombatParticipant target, 
        int weaponId,
        List<int>? forcedRolls = null)
    {
        // Attaque simplifiée pour systèmes génériques
        var attackRoll = forcedRolls?.Count > 0 ? 
            forcedRolls[0] : _diceRoller.RollDie(20);

        var hit = attackRoll + 5 >= target.ArmorClass; // Bonus fixe +5 pour générique
        var damage = hit ? (forcedRolls?.Count > 1 ? forcedRolls[1] : _diceRoller.RollDie(8)) + 2 : 0;

        if (hit && damage > 0)
        {
            target.TakeDamage(damage);
        }

        return new CombatActionResult
        {
            Success = true,
            ActionDescription = hit ? 
                $"{attacker.Name} attaque {target.Name} et inflige {damage} dégâts !" :
                $"{attacker.Name} attaque {target.Name} mais rate !",
            ActorName = attacker.Name,
            TargetName = target.Name,
            DamageDealt = damage,
            TargetDefeated = !target.IsAlive,
            DiceRolls = [new DiceRollResult 
            { 
                DiceType = "1d20", 
                Rolls = [attackRoll], 
                Total = attackRoll + 5, 
                Modifier = 5,
                Purpose = "attack" 
            }]
        };
    }

    /// <summary>
    /// Exécute un sort avec attaque de sort
    /// </summary>
    private async Task<CombatActionResult> ExecuteSpellAttackAsync(CombatParticipant caster, ExecuteCombatActionCommand command)
    {
        // Implementation des sorts d'attaque sera ajoutée prochainement
        return new CombatActionResult
        {
            Success = true,
            ActionDescription = $"{caster.Name} lance un sort d'attaque !",
            ActorName = caster.Name
        };
    }

    /// <summary>
    /// Exécute un sort avec jet de sauvegarde
    /// </summary>
    private async Task<CombatActionResult> ExecuteSpellSaveAsync(CombatParticipant caster, ExecuteCombatActionCommand command)
    {
        // Implementation des sorts de sauvegarde sera ajoutée prochainement
        return new CombatActionResult
        {
            Success = true,
            ActionDescription = $"{caster.Name} lance un sort avec sauvegarde !",
            ActorName = caster.Name
        };
    }

    /// <summary>
    /// Exécute un sort générique (utilitaire, soin, etc.)
    /// </summary>
    private async Task<CombatActionResult> ExecuteSpellCastAsync(CombatParticipant caster, ExecuteCombatActionCommand command)
    {
        // Implementation des sorts génériques sera ajoutée prochainement
        return new CombatActionResult
        {
            Success = true,
            ActionDescription = $"{caster.Name} lance un sort !",
            ActorName = caster.Name
        };
    }

    private async Task<CombatActionResult> ExecuteMoveAsync(CombatParticipant actor, ExecuteCombatActionCommand command)
    {
        var distance = command.MovementDistance ?? actor.Speed;
        var oldPosition = actor.Position ?? "position inconnue";
        actor.Position = command.Position ?? "nouvelle position";

        return new CombatActionResult
        {
            Success = true,
            ActionDescription = $"{actor.Name} se déplace de {oldPosition} vers {actor.Position} ({distance}m)",
            ActorName = actor.Name
        };
    }

    private CombatActionResult ExecuteDodge(CombatParticipant actor)
    {
        // Ajouter un effet temporaire d'esquive (sera géré par les effets de statut plus tard)
        return new CombatActionResult
        {
            Success = true,
            ActionDescription = $"{actor.Name} prend une position défensive (+2 CA jusqu'au prochain tour)",
            ActorName = actor.Name
        };
    }

    private CombatActionResult ExecuteHelp(CombatParticipant actor, ExecuteCombatActionCommand command)
    {
        var targetName = "un allié";
        if (command.TargetId.HasValue)
        {
            var target = _context.CombatParticipants
                .FirstOrDefault(p => p.Id == command.TargetId.Value);
            targetName = target?.Name ?? "un allié";
        }

        return new CombatActionResult
        {
            Success = true,
            ActionDescription = $"{actor.Name} aide {targetName} (avantage sur la prochaine action)",
            ActorName = actor.Name,
            TargetName = targetName
        };
    }

    private CombatActionResult ExecuteEndTurn(CombatParticipant actor)
    {
        return new CombatActionResult
        {
            Success = true,
            ActionDescription = $"{actor.Name} termine son tour",
            ActorName = actor.Name
        };
    }

    #endregion

    #region Méthodes utilitaires privées

    private async Task<CombatParticipant> CreateParticipantFromDtoAsync(int combatId, CombatParticipantDto dto)
    {
        var participant = new CombatParticipant
        {
            CombatId = combatId,
            ParticipantType = dto.ParticipantType,
            CharacterId = dto.CharacterId,
            NpcId = dto.NpcId,
            Name = dto.CustomName ?? "Participant",
            Initiative = dto.CustomInitiative,
            Position = dto.Position,
            IsActive = true,
            IsConscious = true,
            CreatedAt = DateTime.UtcNow
        };

        // Initialiser les stats selon le type
        if (dto.ParticipantType == "player" && dto.CharacterId.HasValue)
        {
            await InitializePlayerStatsAsync(participant, dto.CharacterId.Value);
        }
        else if (dto.ParticipantType == "npc")
        {
            InitializeNpcStats(participant, dto.CustomStats);
        }

        return participant;
    }

    private async Task InitializePlayerStatsAsync(CombatParticipant participant, int characterId)
    {
        // Initialisation simplifiée - en production, récupérer depuis le personnage
        participant.Name = $"Personnage {characterId}";
        participant.CurrentHitPoints = 20;
        participant.MaxHitPoints = 20;
        participant.ArmorClass = 15;
        participant.Speed = 9;
    }

    private void InitializeNpcStats(CombatParticipant participant, Dictionary<string, object>? customStats)
    {
        if (customStats != null)
        {
            participant.CurrentHitPoints = Convert.ToInt32(customStats.GetValueOrDefault("hitPoints", 10));
            participant.MaxHitPoints = participant.CurrentHitPoints;
            participant.ArmorClass = Convert.ToInt32(customStats.GetValueOrDefault("armorClass", 10));
            participant.Speed = Convert.ToInt32(customStats.GetValueOrDefault("speed", 9));
        }
        else
        {
            // Valeurs par défaut
            participant.CurrentHitPoints = 10;
            participant.MaxHitPoints = 10;
            participant.ArmorClass = 10;
            participant.Speed = 9;
        }
    }

    /// <summary>
    /// Récupère un équipement D&D depuis la base de données
    /// </summary>
    private async Task<EquipmentDto?> GetDndEquipmentAsync(int equipmentId)
    {
        // Utiliser une requête SQL brute pour accéder aux équipements D&D
        // depuis le contexte commun (contournement temporaire)
        try
        {
            var equipment = await _context.Database.SqlQueryRaw<EquipmentDto>(
                "SELECT Id, Name, IsWeapon, Damage, DamageType, Properties, WeaponType, Tags FROM EquipmentDnd WHERE Id = {0}",
                equipmentId).FirstOrDefaultAsync();

            return equipment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'équipement {EquipmentId}", equipmentId);
            return null;
        }
    }

    /// <summary>
    /// Détermine le type d'avantage pour une attaque
    /// </summary>
    private string DetermineAdvantageType(CombatParticipant attacker, CombatParticipant target, EquipmentDto weapon)
    {
        // Logic simplifiée - en production, vérifier les conditions, effets de statut, etc.
        
        // Exemple de conditions d'avantage
        if (target.Position?.Contains("prone") == true) return "advantage";
        if (attacker.Position?.Contains("flanking") == true) return "advantage";
        
        // Exemple de conditions de désavantage
        if (attacker.StatusEffects?.Contains("poisoned") == true) return "disadvantage";
        
        return "normal";
    }

    /// <summary>
    /// Crée un jet d'attaque avec dés forcés (pour les tests)
    /// </summary>
    private DiceRollResult CreateForcedAttackRoll(int roll, int attackBonus)
    {
        return new DiceRollResult
        {
            DiceType = "1d20",
            Rolls = [roll],
            Modifier = attackBonus,
            Total = roll + attackBonus,
            Purpose = "attack",
            AdvantageType = "normal"
        };
    }

    /// <summary>
    /// Crée un jet de dégâts avec dés forcés (pour les tests)
    /// </summary>
    private DiceRollResult CreateForcedDamageRoll(List<int> rolls, int modifier, string formula)
    {
        return new DiceRollResult
        {
            DiceType = formula,
            Rolls = rolls,
            Modifier = modifier,
            Total = rolls.Sum() + modifier,
            Purpose = "damage",
            AdvantageType = "normal"
        };
    }

    /// <summary>
    /// Génère une description détaillée d'attaque d'arme
    /// </summary>
    private string GenerateWeaponAttackDescription(
        CombatParticipant attacker, 
        CombatParticipant target, 
        EquipmentDto weapon, 
        DiceRollResult attackRoll, 
        DiceRollResult damageRoll, 
        bool isCritical)
    {
        var description = new StringBuilder();
        
        // Début de l'action
        description.Append($"⚔️ {attacker.Name} attaque {target.Name} avec {weapon.Name}");
        
        // Détails du jet d'attaque
        if (isCritical)
        {
            description.Append($" !\n🎯 COUP CRITIQUE ! (Jet naturel 20)");
        }
        else
        {
            description.Append($" !\n🎲 Jet d'attaque: {attackRoll.DiceType}+{attackRoll.Modifier} = {attackRoll.Total}");
            description.Append($" (touche CA {target.ArmorClass})");
        }
        
        // Dégâts infligés
        if (isCritical)
        {
            description.Append($"\n💥 Dégâts critiques: {damageRoll.DiceType}+{damageRoll.Modifier} = {damageRoll.Total} dégâts {weapon.DamageType}");
        }
        else
        {
            description.Append($"\n⚡ Dégâts: {damageRoll.DiceType}+{damageRoll.Modifier} = {damageRoll.Total} dégâts {weapon.DamageType}");
        }
        
        // État de la cible
        if (target.CurrentHitPoints <= 0)
        {
            description.Append($"\n💀 {target.Name} est éliminé !");
        }
        else
        {
            description.Append($"\n🩸 {target.Name}: {target.CurrentHitPoints}/{target.MaxHitPoints} PV restants");
        }
        
        return description.ToString();
    }

    private CombatParticipantStateDto MapToParticipantState(CombatParticipant participant)
    {
        return new CombatParticipantStateDto
        {
            Id = participant.Id,
            Name = participant.Name,
            ParticipantType = participant.ParticipantType,
            Initiative = participant.Initiative,
            CurrentHitPoints = participant.CurrentHitPoints,
            MaxHitPoints = participant.MaxHitPoints,
            TemporaryHitPoints = participant.TemporaryHitPoints,
            ArmorClass = participant.ArmorClass,
            Speed = participant.Speed,
            IsActive = participant.IsActive,
            IsConscious = participant.IsConscious,
            Position = participant.Position,
            HealthPercentage = participant.HealthPercentage,
            HealthStatus = participant.HealthStatus,
            StatusEffects = ParseStatusEffects(participant.StatusEffects),
            Resources = ParseResources(participant.Resources),
            AvailableActions = ParseAvailableActions(participant.AvailableActions)
        };
    }

    private CombatEnvironmentDto? ParseEnvironment(string? environmentJson)
    {
        if (string.IsNullOrEmpty(environmentJson)) return null;
        
        try
        {
            return JsonSerializer.Deserialize<CombatEnvironmentDto>(environmentJson);
        }
        catch
        {
            return null;
        }
    }

    private List<string> GetRecentActionLog(List<CombatAction> actions)
    {
        return actions
            .OrderByDescending(a => a.ExecutedAt)
            .Take(10)
            .Select(a => a.GenerateLogMessage())
            .ToList();
    }

    private TimeSpan? CalculateTimeRemaining(CombatSession combat)
    {
        if (combat.TurnTimeLimit == null || combat.CurrentTurnStarted == null)
            return null;

        var elapsed = DateTime.UtcNow - combat.CurrentTurnStarted.Value;
        var remaining = combat.TurnTimeLimit.Value - elapsed;

        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    private string DetermineWinner(List<CombatParticipant> participants)
    {
        var activePlayers = participants.Count(p => p.ParticipantType == "player" && p.CurrentHitPoints > 0);
        var activeNpcs = participants.Count(p => p.ParticipantType == "npc" && p.CurrentHitPoints > 0);

        if (activePlayers > 0 && activeNpcs == 0) return "Joueurs";
        if (activeNpcs > 0 && activePlayers == 0) return "PNJ";
        return "Match nul";
    }

    private List<StatusEffectDto> ParseStatusEffects(string? statusEffectsJson)
    {
        if (string.IsNullOrEmpty(statusEffectsJson)) return new List<StatusEffectDto>();
        
        try
        {
            return JsonSerializer.Deserialize<List<StatusEffectDto>>(statusEffectsJson) ?? new List<StatusEffectDto>();
        }
        catch
        {
            return new List<StatusEffectDto>();
        }
    }

    private Dictionary<string, object>? ParseResources(string? resourcesJson)
    {
        if (string.IsNullOrEmpty(resourcesJson)) return null;
        
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(resourcesJson);
        }
        catch
        {
            return null;
        }
    }

    private List<string> ParseAvailableActions(string? actionsJson)
    {
        if (string.IsNullOrEmpty(actionsJson)) 
            return new List<string> { "weapon_attack", "move", "dodge", "help", "end_turn" };
        
        try
        {
            return JsonSerializer.Deserialize<List<string>>(actionsJson) ?? 
                   new List<string> { "weapon_attack", "move", "dodge", "help", "end_turn" };
        }
        catch
        {
            return new List<string> { "weapon_attack", "move", "dodge", "help", "end_turn" };
        }
    }

    #endregion
}

/// <summary>
/// DTO temporaire pour récupérer les équipements via SQL brut
/// </summary>
public class EquipmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsWeapon { get; set; }
    public string Damage { get; set; } = string.Empty;
    public string DamageType { get; set; } = string.Empty;
    public string Properties { get; set; } = string.Empty;
    public string WeaponType { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
}