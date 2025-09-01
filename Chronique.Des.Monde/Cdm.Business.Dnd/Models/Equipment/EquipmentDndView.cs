using Cmd.Abstraction.Equipment;
using Cdm.Common.Enums;

namespace Cdm.Business.Dnd.Models.Equipment;

/// <summary>
/// Modèle de vue pour afficher un équipement D&D avec toutes ses propriétés
/// Implémente IEquipmentView pour l'abstraction
/// </summary>
public record EquipmentDndView : IEquipmentView
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
    public string Category { get; init; } = string.Empty;
    public decimal Weight { get; init; }
    public decimal Value { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    
    // Propriétés D&D spécifiques
    public string WeaponType { get; init; } = string.Empty;
    public string Damage { get; init; } = string.Empty;
    public string DamageType { get; init; } = string.Empty;
    public int AttackBonus { get; init; }
    public string Properties { get; init; } = string.Empty;
    public int ArmorClass { get; init; }
    public int MaxDexBonus { get; init; }
    public bool StealthDisadvantage { get; init; }
    public int StrengthRequirement { get; init; }
    public bool IsWeapon { get; init; }
    public bool IsArmor { get; init; }
    public bool IsShield { get; init; }
    public bool IsMagical { get; init; }
    public bool RequiresAttunement { get; init; }
    public string Rarity { get; init; } = string.Empty;
    public string? MagicalProperties { get; init; }
    public int? Charges { get; init; }
    public bool IsConsumable { get; init; }
    public string? Effect { get; init; }
}