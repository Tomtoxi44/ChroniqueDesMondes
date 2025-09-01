namespace Cdm.Business.Dnd.Models.Spells;

/// <summary>
/// Modèle de requête pour créer un sort D&D spécialisé
/// </summary>
public record SpellDndRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public List<string>? Tags { get; init; }
    
    // Propriétés D&D spécifiques
    public string School { get; init; } = string.Empty;
    public int Level { get; init; } = 1;
    public string CastingTime { get; init; } = string.Empty;
    public string Range { get; init; } = string.Empty;
    public string Duration { get; init; } = string.Empty;
    public string Components { get; init; } = string.Empty;
    public string? AttackRoll { get; init; }
    public string? Damage { get; init; }
    public string? SavingThrow { get; init; }
    public bool IsRitual { get; init; } = false;
    public bool RequiresConcentration { get; init; } = false;
    public string? MaterialComponent { get; init; }
    public int? HigherLevelDamage { get; init; }
}