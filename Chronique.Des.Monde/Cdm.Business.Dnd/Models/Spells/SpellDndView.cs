using Cdm.Business.Dnd.Models.Spells;
using Cdm.Common.Enums;

namespace Cdm.Business.Dnd.Models.Spells;

/// <summary>
/// Vue spécialisée pour les sorts D&D avec toutes les propriétés spécifiques
/// </summary>
public record SpellDndView
{
    // Propriétés de base
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public GameType GameType { get; init; }
    public bool IsPublic { get; init; }
    public string Source { get; init; } = string.Empty;
    public string CreatedByName { get; init; } = string.Empty;
    public List<string> Tags { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    // Propriétés spécifiques D&D
    public string School { get; init; } = string.Empty;
    public int Level { get; init; }
    public string CastingTime { get; init; } = string.Empty;
    public string Range { get; init; } = string.Empty;
    public string Duration { get; init; } = string.Empty;
    public string Components { get; init; } = string.Empty;
    public string AttackRoll { get; init; } = string.Empty;
    public string Damage { get; init; } = string.Empty;
    public string SavingThrow { get; init; } = string.Empty;
    public bool IsRitual { get; init; }
    public bool RequiresConcentration { get; init; }
    public string MaterialComponent { get; init; } = string.Empty;
    public int? HigherLevelDamage { get; init; }
    
    // Nouvelles propriétés ajoutées
    public string DamageType { get; init; } = string.Empty;
    public string SaveType { get; init; } = string.Empty;
    public string AttackType { get; init; } = string.Empty;
}