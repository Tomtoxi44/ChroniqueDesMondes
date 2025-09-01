using Cmd.Abstraction.Spells;
using Cdm.Common.Enums;

namespace Cdm.Business.Dnd.Models.Spells;

/// <summary>
/// Modèle de vue pour afficher un sort D&D avec toutes ses propriétés
/// Implémente ISpellView pour l'abstraction
/// </summary>
public record SpellDndView : ISpellView
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public GameType GameType { get; init; } = GameType.DnD;
    public bool IsPublic { get; init; }
    public string Source { get; init; } = string.Empty;
    public string? CreatedByName { get; init; }
    public List<string> Tags { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    
    // Propriétés D&D spécifiques
    public string School { get; init; } = string.Empty;
    public int Level { get; init; }
    public string CastingTime { get; init; } = string.Empty;
    public string Range { get; init; } = string.Empty;
    public string Duration { get; init; } = string.Empty;
    public string Components { get; init; } = string.Empty;
    public string? AttackRoll { get; init; }
    public string? Damage { get; init; }
    public string? SavingThrow { get; init; }
    public bool IsRitual { get; init; }
    public bool RequiresConcentration { get; init; }
    public string? MaterialComponent { get; init; }
    public int? HigherLevelDamage { get; init; }
}