namespace Cdm.Business.Dnd.Models.Equipment;

/// <summary>
/// Modèle de requête pour créer un équipement D&D spécialisé
/// </summary>
public record EquipmentDndRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public List<string>? Tags { get; init; }
    public string Category { get; init; } = string.Empty;
    public decimal Weight { get; init; } = 0;
    public decimal Value { get; init; } = 0;
    
    // Propriétés D&D spécifiques
    public string WeaponType { get; init; } = string.Empty;
    public string Damage { get; init; } = string.Empty;
    public string DamageType { get; init; } = string.Empty;
    public int AttackBonus { get; init; } = 0;
    public string Properties { get; init; } = string.Empty;
    public int ArmorClass { get; init; } = 0;
    public int MaxDexBonus { get; init; } = -1;
    public bool StealthDisadvantage { get; init; } = false;
    public int StrengthRequirement { get; init; } = 0;
    public bool IsWeapon { get; init; } = false;
    public bool IsArmor { get; init; } = false;
    public bool IsShield { get; init; } = false;
    public bool IsMagical { get; init; } = false;
    public bool RequiresAttunement { get; init; } = false;
    public string Rarity { get; init; } = "Common";
    public string? MagicalProperties { get; init; }
    public int? Charges { get; init; }
    public bool IsConsumable { get; init; } = false;
    public string? Effect { get; init; }
}