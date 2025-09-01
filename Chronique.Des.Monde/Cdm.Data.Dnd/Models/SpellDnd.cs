using System.ComponentModel.DataAnnotations;
using Cdm.Data.Models;

namespace Cdm.Data.Dnd.Models;

/// <summary>
/// Sort D&D avec propriétés spécialisées, hérite d'ASpell
/// Suit le pattern CharacterDnd héritant d'ACharacter
/// </summary>
public class SpellDnd : ASpell
{
    [MaxLength(50)]
    public string School { get; set; } = string.Empty; // Ex: "Évocation", "Enchantement"

    [Required]
    [Range(0, 9)]
    public int Level { get; set; } = 1; // Niveau du sort (0-9)

    [MaxLength(100)]
    public string CastingTime { get; set; } = string.Empty; // Ex: "1 action", "1 minute"

    [MaxLength(100)]
    public string Range { get; set; } = string.Empty; // Ex: "30 mètres", "Contact"

    [MaxLength(200)]
    public string Duration { get; set; } = string.Empty; // Ex: "Instantané", "Concentration, 1 minute"

    [MaxLength(50)]
    public string Components { get; set; } = string.Empty; // Ex: "V, S, M"

    [MaxLength(100)]
    public string AttackRoll { get; set; } = string.Empty; // Ex: "1d20 + INT", "" si pas d'attaque

    [MaxLength(100)]
    public string Damage { get; set; } = string.Empty; // Ex: "2d8 + INT", "" si pas de dégâts

    [MaxLength(50)]
    public string SavingThrow { get; set; } = string.Empty; // Ex: "DEX", "WIS", "" si pas de sauvegarde

    public bool IsRitual { get; set; } = false; // Peut être lancé en rituel

    public bool RequiresConcentration { get; set; } = false; // Nécessite concentration

    [MaxLength(500)]
    public string MaterialComponent { get; set; } = string.Empty; // Description des composantes matérielles

    public int? HigherLevelDamage { get; set; } // Dégâts supplémentaires par niveau
}