using Cdm.Common.Enums;

namespace Cdm.Business.Common.Models.Combat;

/// <summary>
/// DTO pour démarrer un nouveau combat
/// </summary>
public class StartCombatCommand
{
    public string SessionId { get; set; } = string.Empty;
    public int ChapterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public GameType GameType { get; set; }
    public List<CombatParticipantDto> Participants { get; set; } = new();
    public CombatEnvironmentDto? Environment { get; set; }
    public CombatSettingsDto? Settings { get; set; }
}

/// <summary>
/// DTO pour un participant au combat
/// </summary>
public class CombatParticipantDto
{
    public string ParticipantType { get; set; } = string.Empty; // "player" ou "npc"
    public int? CharacterId { get; set; }
    public int? NpcId { get; set; }
    public string? CustomName { get; set; } // Pour surcharger le nom
    public int? CustomInitiative { get; set; } // Pour forcer une initiative
    public string? Position { get; set; }
    public Dictionary<string, object>? CustomStats { get; set; } // Stats personnalisées pour les PNJ
}

/// <summary>
/// DTO pour l'environnement de combat
/// </summary>
public class CombatEnvironmentDto
{
    public string? Terrain { get; set; } // "forest", "dungeon", "city", etc.
    public string? Lighting { get; set; } // "bright", "dim", "dark"
    public string? Weather { get; set; } // "clear", "rain", "fog", etc.
    public List<string>? Features { get; set; } // ["difficult_terrain", "cover", "hazards"]
    public string? Description { get; set; }
    public Dictionary<string, object>? Properties { get; set; } // Propriétés spécifiques
}

/// <summary>
/// DTO pour les paramètres de combat
/// </summary>
public class CombatSettingsDto
{
    public TimeSpan? TurnTimeLimit { get; set; }
    public bool AutoAdvanceTurns { get; set; } = false;
    public bool AllowLateJoin { get; set; } = true;
    public bool ShowInitiativeToPlayers { get; set; } = true;
    public bool EnableCriticalConfirmation { get; set; } = false;
    public Dictionary<string, object>? HouseRules { get; set; }
}

/// <summary>
/// Commande pour exécuter une action de combat
/// </summary>
public class ExecuteCombatActionCommand
{
    public int CombatId { get; set; }
    public int ActorId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public int? TargetId { get; set; }
    public int? EquipmentId { get; set; }
    public int? SpellId { get; set; }
    public int? SpellLevel { get; set; }
    public string? Position { get; set; }
    public int? MovementDistance { get; set; }
    public List<int>? DiceRolls { get; set; } // Jets pré-calculés (pour les tests)
    public Dictionary<string, object>? AdditionalData { get; set; }
}

/// <summary>
/// Résultat d'une action de combat
/// </summary>
public class CombatActionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string ActionDescription { get; set; } = string.Empty;
    public string ActorName { get; set; } = string.Empty;
    public string? TargetName { get; set; }
    public List<DiceRollResult> DiceRolls { get; set; } = new();
    public int? DamageDealt { get; set; }
    public string? DamageType { get; set; }
    public int? HealingDone { get; set; }
    public bool IsCritical { get; set; }
    public bool TargetDefeated { get; set; }
    public List<StatusEffectResult> StatusEffects { get; set; } = new();
    public CombatParticipantStateDto? UpdatedActor { get; set; }
    public CombatParticipantStateDto? UpdatedTarget { get; set; }
    public bool CombatEnded { get; set; }
    public string? Winner { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}

/// <summary>
/// Résultat d'un jet de dé
/// </summary>
public class DiceRollResult
{
    public string DiceType { get; set; } = string.Empty; // "1d20", "2d6", etc.
    public List<int> Rolls { get; set; } = new();
    public int Modifier { get; set; }
    public int Total { get; set; }
    public string Purpose { get; set; } = string.Empty; // "attack", "damage", "save"
    public string? AdvantageType { get; set; } // "advantage", "disadvantage", "normal"
}

/// <summary>
/// Résultat d'application d'effet de statut
/// </summary>
public class StatusEffectResult
{
    public string EffectName { get; set; } = string.Empty;
    public string TargetName { get; set; } = string.Empty;
    public bool Applied { get; set; }
    public int Duration { get; set; }
    public string? Reason { get; set; }
}

/// <summary>
/// État actuel d'un participant de combat
/// </summary>
public class CombatParticipantStateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ParticipantType { get; set; } = string.Empty;
    public int? Initiative { get; set; }
    public int CurrentHitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int TemporaryHitPoints { get; set; }
    public int ArmorClass { get; set; }
    public int Speed { get; set; }
    public bool IsActive { get; set; }
    public bool IsConscious { get; set; }
    public string? Position { get; set; }
    public List<StatusEffectDto> StatusEffects { get; set; } = new();
    public Dictionary<string, object>? Resources { get; set; }
    public List<string> AvailableActions { get; set; } = new();
    public int HealthPercentage { get; set; }
    public string HealthStatus { get; set; } = string.Empty;
}

/// <summary>
/// DTO pour un effet de statut
/// </summary>
public class StatusEffectDto
{
    public string Name { get; set; } = string.Empty;
    public string EffectType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Duration { get; set; }
    public int Intensity { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool CanBeDispelled { get; set; }
}

/// <summary>
/// DTO pour l'état complet du combat
/// </summary>
public class CombatStateDto
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int CurrentRound { get; set; }
    public int CurrentTurnIndex { get; set; }
    public List<CombatParticipantStateDto> Participants { get; set; } = new();
    public List<InitiativeOrderDto> TurnOrder { get; set; } = new();
    public CombatParticipantStateDto? CurrentParticipant { get; set; }
    public CombatEnvironmentDto? Environment { get; set; }
    public List<string> ActionLog { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public TimeSpan? TurnTimeLimit { get; set; }
    public DateTime? CurrentTurnStarted { get; set; }
    public TimeSpan? TimeRemaining { get; set; }
}

/// <summary>
/// DTO pour l'ordre d'initiative
/// </summary>
public class InitiativeOrderDto
{
    public int ParticipantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ParticipantType { get; set; } = string.Empty;
    public int Initiative { get; set; }
    public bool IsActive { get; set; }
    public bool IsCurrent { get; set; }
    public int HealthPercentage { get; set; }
    public List<StatusEffectDto> StatusEffects { get; set; } = new();
}