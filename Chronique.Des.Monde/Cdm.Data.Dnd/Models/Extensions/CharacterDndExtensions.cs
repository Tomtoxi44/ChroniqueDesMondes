using Cdm.Common.DnD;
using Cdm.Common.Enums;

namespace Cdm.Data.Dnd.Models.Extensions;

/// <summary>
/// Extensions pour ajouter les calculs automatiques D&D aux personnages
/// </summary>
public static class CharacterDndExtensions
{
    /// <summary>
    /// Calcule la Force totale (base + bonus additionnels)
    /// </summary>
    public static int TotalStrength(this CharacterDnd character)
    {
        return character.Strong + character.AdditionalStrong;
    }
    
    /// <summary>
    /// Calcule la Dextérité totale (base + bonus additionnels)
    /// </summary>
    public static int TotalDexterity(this CharacterDnd character)
    {
        return character.Dexterity + character.AdditionalDexterity;
    }
    
    /// <summary>
    /// Calcule la Constitution totale (base + bonus additionnels)
    /// </summary>
    public static int TotalConstitution(this CharacterDnd character)
    {
        return character.Constitution + character.AdditionalConstitution;
    }
    
    /// <summary>
    /// Calcule l'Intelligence totale (base + bonus additionnels)
    /// </summary>
    public static int TotalIntelligence(this CharacterDnd character)
    {
        return character.Intelligence + character.AdditionalIntelligence;
    }
    
    /// <summary>
    /// Calcule la Sagesse totale (base + bonus additionnels)
    /// </summary>
    public static int TotalWisdom(this CharacterDnd character)
    {
        return character.Wisdoms + character.AdditionalWisdoms;
    }
    
    /// <summary>
    /// Calcule le Charisme total (base + bonus additionnels)
    /// </summary>
    public static int TotalCharisma(this CharacterDnd character)
    {
        return character.Charism + character.AdditionalCharism;
    }
    
    /// <summary>
    /// Obtient le score de la caractéristique de lancement de sorts pour ce personnage
    /// </summary>
    public static int GetSpellcastingAbilityScore(this CharacterDnd character)
    {
        if (!DndSpellcastingRules.IsSpellcaster(character.Class))
            throw new InvalidOperationException($"La classe '{character.Class}' ne peut pas lancer de sorts");
            
        var spellcastingAbility = DndSpellcastingRules.GetSpellcastingAbility(character.Class);
        
        return spellcastingAbility switch
        {
            SpellcastingAbility.Intelligence => character.TotalIntelligence(),
            SpellcastingAbility.Wisdom => character.TotalWisdom(),
            SpellcastingAbility.Charisma => character.TotalCharisma(),
            _ => throw new ArgumentOutOfRangeException(nameof(spellcastingAbility))
        };
    }
    
    /// <summary>
    /// Calcule le modificateur de Force
    /// </summary>
    public static int StrengthModifier(this CharacterDnd character)
    {
        return DndSpellcastingRules.CalculateAbilityModifier(character.TotalStrength());
    }
    
    /// <summary>
    /// Calcule le modificateur de Dextérité
    /// </summary>
    public static int DexterityModifier(this CharacterDnd character)
    {
        return DndSpellcastingRules.CalculateAbilityModifier(character.TotalDexterity());
    }
    
    /// <summary>
    /// Calcule le modificateur de Constitution
    /// </summary>
    public static int ConstitutionModifier(this CharacterDnd character)
    {
        return DndSpellcastingRules.CalculateAbilityModifier(character.TotalConstitution());
    }
    
    /// <summary>
    /// Calcule le modificateur d'Intelligence
    /// </summary>
    public static int IntelligenceModifier(this CharacterDnd character)
    {
        return DndSpellcastingRules.CalculateAbilityModifier(character.TotalIntelligence());
    }
    
    /// <summary>
    /// Calcule le modificateur de Sagesse
    /// </summary>
    public static int WisdomModifier(this CharacterDnd character)
    {
        return DndSpellcastingRules.CalculateAbilityModifier(character.TotalWisdom());
    }
    
    /// <summary>
    /// Calcule le modificateur de Charisme
    /// </summary>
    public static int CharismaModifier(this CharacterDnd character)
    {
        return DndSpellcastingRules.CalculateAbilityModifier(character.TotalCharisma());
    }
    
    /// <summary>
    /// Calcule le bonus de maîtrise du personnage
    /// </summary>
    public static int ProficiencyBonus(this CharacterDnd character)
    {
        return DndSpellcastingRules.CalculateProficiencyBonus(character.Leveling);
    }
    
    /// <summary>
    /// Calcule les statistiques complètes de lancement de sorts
    /// </summary>
    public static SpellcastingStats GetSpellcastingStats(this CharacterDnd character)
    {
        if (!DndSpellcastingRules.IsSpellcaster(character.Class))
            throw new InvalidOperationException($"La classe '{character.Class}' ne peut pas lancer de sorts");
            
        var spellcastingAbilityScore = character.GetSpellcastingAbilityScore();
        
        return DndSpellcastingRules.CalculateSpellcastingStats(
            character.Class, 
            character.Leveling, 
            spellcastingAbilityScore);
    }
    
    /// <summary>
    /// Vérifie si le personnage peut lancer des sorts
    /// </summary>
    public static bool CanCastSpells(this CharacterDnd character)
    {
        return DndSpellcastingRules.IsSpellcaster(character.Class);
    }
    
    /// <summary>
    /// Calcule la Classe d'Armure (CA) du personnage
    /// </summary>
    public static int CalculateArmorClass(this CharacterDnd character)
    {
        // CA de base (10 + mod Dextérité + Armure)
        var baseAC = 10;
        var dexModifier = character.DexterityModifier();
        var armorAC = character.ClassArmor; // L'armure portée
        
        return baseAC + dexModifier + armorAC;
    }
    
    /// <summary>
    /// Calcule les Points de Vie du personnage
    /// </summary>
    public static int CalculateHitPoints(this CharacterDnd character)
    {
        // PV = PV de base classe + mod Constitution * niveau
        var baseHP = GetClassBaseHP(character.Class);
        var conModifier = character.ConstitutionModifier();
        var levelBonus = (character.Leveling - 1) * (GetClassHitDie(character.Class) / 2 + 1);
        
        return baseHP + conModifier + levelBonus + (conModifier * (character.Leveling - 1));
    }
    
    /// <summary>
    /// Obtient les PV de base selon la classe D&D
    /// </summary>
    private static int GetClassBaseHP(string characterClass)
    {
        return characterClass.ToLower() switch
        {
            "magicien" or "wizard" => 6,       // d6
            "ensorceleur" or "sorcerer" => 6,  // d6
            "barde" or "bard" => 8,            // d8
            "clerc" or "cleric" => 8,          // d8
            "druide" or "druid" => 8,          // d8
            "rôdeur" or "ranger" => 10,        // d10
            "paladin" => 10,                   // d10
            "occultiste" or "warlock" => 8,    // d8
            _ => 8 // Valeur par défaut
        };
    }
    
    /// <summary>
    /// Obtient le dé de vie selon la classe D&D
    /// </summary>
    private static int GetClassHitDie(string characterClass)
    {
        return characterClass.ToLower() switch
        {
            "magicien" or "wizard" => 6,       // d6
            "ensorceleur" or "sorcerer" => 6,  // d6
            "barde" or "bard" => 8,            // d8
            "clerc" or "cleric" => 8,          // d8
            "druide" or "druid" => 8,          // d8
            "rôdeur" or "ranger" => 10,        // d10
            "paladin" => 10,                   // d10
            "occultiste" or "warlock" => 8,    // d8
            _ => 8 // Valeur par défaut
        };
    }
}