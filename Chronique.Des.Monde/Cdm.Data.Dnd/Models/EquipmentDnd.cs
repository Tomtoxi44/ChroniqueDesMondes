using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cdm.Data.Models;

namespace Cdm.Data.Dnd.Models;

/// <summary>
/// Équipement D&D avec propriétés spécialisées, hérite d'AEquipment
/// Suit le pattern SpellDnd héritant d'ASpell
/// </summary>
public class EquipmentDnd : AEquipment
{
    // Propriétés d'arme
    [MaxLength(50)]
    public string WeaponType { get; set; } = string.Empty; // Ex: "Sword", "Bow", "Mace"

    [MaxLength(100)]
    public string Damage { get; set; } = string.Empty; // Ex: "1d8 + STR", "1d6"

    [MaxLength(50)]
    public string DamageType { get; set; } = string.Empty; // Ex: "Slashing", "Piercing", "Bludgeoning"

    public int AttackBonus { get; set; } = 0; // Bonus d'attaque magique (+1, +2, etc.)

    [MaxLength(100)]
    public string Properties { get; set; } = string.Empty; // Ex: "Versatile", "Light", "Heavy"

    // Propriétés d'armure
    public int ArmorClass { get; set; } = 0; // CA de base de l'armure

    public int MaxDexBonus { get; set; } = -1; // -1 = pas de limite, 0+ = limite du bonus DEX

    public bool StealthDisadvantage { get; set; } = false; // Désavantage en discrétion

    public int StrengthRequirement { get; set; } = 0; // Force minimum requise

    // Propriétés générales
    public bool IsWeapon { get; set; } = false;

    public bool IsArmor { get; set; } = false;

    public bool IsShield { get; set; } = false;

    public bool IsMagical { get; set; } = false;

    public bool RequiresAttunement { get; set; } = false;

    [MaxLength(50)]
    public string Rarity { get; set; } = "Common"; // Common, Uncommon, Rare, Very Rare, Legendary

    [Column(TypeName = "nvarchar(max)")]
    public string? MagicalProperties { get; set; } // Description des propriétés magiques

    // Propriétés spécifiques aux consommables
    public int? Charges { get; set; } // Nombre d'utilisations (potions, parchemins, etc.)

    public bool IsConsumable { get; set; } = false;

    [MaxLength(200)]
    public string? Effect { get; set; } // Effet du consommable
}