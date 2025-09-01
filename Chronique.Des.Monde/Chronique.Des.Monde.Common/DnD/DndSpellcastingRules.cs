using Cdm.Common.Enums;

namespace Cdm.Common.DnD;

/// <summary>
/// Règles de calcul automatiques pour D&D 5e - Lancement de sorts et modificateurs
/// Implémente les règles officielles du System Reference Document
/// </summary>
public static class DndSpellcastingRules
{
    /// <summary>
    /// Détermine la caractéristique de lancement de sorts selon la classe D&D
    /// </summary>
    public static SpellcastingAbility GetSpellcastingAbility(string characterClass)
    {
        return characterClass.ToLower() switch
        {
            // Intelligence - Classes érudites
            "magicien" or "wizard" => SpellcastingAbility.Intelligence,
            "occultiste" or "warlock" => SpellcastingAbility.Intelligence,
            
            // Sagesse - Classes divines/nature
            "clerc" or "cleric" => SpellcastingAbility.Wisdom,
            "druide" or "druid" => SpellcastingAbility.Wisdom,
            "rôdeur" or "ranger" => SpellcastingAbility.Wisdom,
            
            // Charisme - Classes innées/sociales
            "barde" or "bard" => SpellcastingAbility.Charisma,
            "ensorceleur" or "sorcerer" => SpellcastingAbility.Charisma,
            "paladin" => SpellcastingAbility.Charisma,
            
            // Classe non reconnue
            _ => throw new ArgumentException($"La classe '{characterClass}' n'est pas reconnue comme lanceur de sorts D&D 5e")
        };
    }
    
    /// <summary>
    /// Calcule le modificateur de caractéristique selon les règles D&D 5e
    /// Formule : (score - 10) / 2 (arrondi vers le bas)
    /// </summary>
    public static int CalculateAbilityModifier(int abilityScore)
    {
        if (abilityScore < 1 || abilityScore > 30)
            throw new ArgumentOutOfRangeException(nameof(abilityScore), "Le score de caractéristique doit être entre 1 et 30");
            
        return (abilityScore - 10) / 2;
    }
    
    /// <summary>
    /// Calcule le bonus d'attaque de sort selon les règles D&D 5e
    /// Formule : modificateur de caractéristique + bonus de maîtrise
    /// </summary>
    public static int CalculateSpellAttackBonus(int abilityScore, int proficiencyBonus)
    {
        var abilityModifier = CalculateAbilityModifier(abilityScore);
        return abilityModifier + proficiencyBonus;
    }
    
    /// <summary>
    /// Calcule le DD de sauvegarde contre les sorts selon les règles D&D 5e
    /// Formule : 8 + modificateur de caractéristique + bonus de maîtrise
    /// </summary>
    public static int CalculateSpellSaveDC(int abilityScore, int proficiencyBonus)
    {
        var abilityModifier = CalculateAbilityModifier(abilityScore);
        return 8 + abilityModifier + proficiencyBonus;
    }
    
    /// <summary>
    /// Calcule le bonus de maîtrise selon le niveau du personnage D&D 5e
    /// Progression officielle : +2 (niv 1-4), +3 (niv 5-8), +4 (niv 9-12), +5 (niv 13-16), +6 (niv 17-20)
    /// </summary>
    public static int CalculateProficiencyBonus(int characterLevel)
    {
        if (characterLevel < 1 || characterLevel > 20)
            throw new ArgumentOutOfRangeException(nameof(characterLevel), "Le niveau de personnage doit être entre 1 et 20");
            
        return (characterLevel - 1) / 4 + 2;
    }
    
    /// <summary>
    /// Détermine le niveau maximum de sort qu'un personnage peut lancer selon son niveau de classe
    /// </summary>
    public static int GetMaxSpellLevel(int characterLevel)
    {
        return characterLevel switch
        {
            1 => 1,                    // Niveau 1 : sorts de niveau 1
            >= 2 and <= 2 => 1,       // Niveau 2 : sorts de niveau 1
            >= 3 and <= 4 => 2,       // Niveaux 3-4 : sorts de niveau 2
            >= 5 and <= 6 => 3,       // Niveaux 5-6 : sorts de niveau 3
            >= 7 and <= 8 => 4,       // Niveaux 7-8 : sorts de niveau 4
            >= 9 and <= 10 => 5,      // Niveaux 9-10 : sorts de niveau 5
            >= 11 and <= 12 => 6,     // Niveaux 11-12 : sorts de niveau 6
            >= 13 and <= 14 => 7,     // Niveaux 13-14 : sorts de niveau 7
            >= 15 and <= 16 => 8,     // Niveaux 15-16 : sorts de niveau 8
            >= 17 and <= 20 => 9,     // Niveaux 17-20 : sorts de niveau 9
            _ => throw new ArgumentOutOfRangeException(nameof(characterLevel), "Le niveau de personnage doit être entre 1 et 20")
        };
    }
    
    /// <summary>
    /// Vérifie si une classe peut lancer des sorts
    /// </summary>
    public static bool IsSpellcaster(string characterClass)
    {
        try
        {
            GetSpellcastingAbility(characterClass);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
    
    /// <summary>
    /// Calcule les statistiques de lancement de sorts complètes pour un personnage D&D
    /// </summary>
    public static SpellcastingStats CalculateSpellcastingStats(string characterClass, int characterLevel, int abilityScore)
    {
        if (!IsSpellcaster(characterClass))
            throw new ArgumentException($"La classe '{characterClass}' ne peut pas lancer de sorts");
            
        var spellcastingAbility = GetSpellcastingAbility(characterClass);
        var proficiencyBonus = CalculateProficiencyBonus(characterLevel);
        var abilityModifier = CalculateAbilityModifier(abilityScore);
        var spellAttackBonus = CalculateSpellAttackBonus(abilityScore, proficiencyBonus);
        var spellSaveDC = CalculateSpellSaveDC(abilityScore, proficiencyBonus);
        var maxSpellLevel = GetMaxSpellLevel(characterLevel);
        
        return new SpellcastingStats
        {
            SpellcastingAbility = spellcastingAbility,
            AbilityModifier = abilityModifier,
            ProficiencyBonus = proficiencyBonus,
            SpellAttackBonus = spellAttackBonus,
            SpellSaveDC = spellSaveDC,
            MaxSpellLevel = maxSpellLevel
        };
    }
}

/// <summary>
/// Statistiques calculées pour le lancement de sorts d'un personnage
/// </summary>
public record SpellcastingStats
{
    public SpellcastingAbility SpellcastingAbility { get; init; }
    public int AbilityModifier { get; init; }
    public int ProficiencyBonus { get; init; }
    public int SpellAttackBonus { get; init; }
    public int SpellSaveDC { get; init; }
    public int MaxSpellLevel { get; init; }
}