using Xunit;
using Cdm.Common.DnD;
using Cdm.Common.Enums;

namespace Cdm.Tests.Common.DnD;

/// <summary>
/// Tests unitaires pour les règles de lancement de sorts D&D 5e
/// Valide les calculs automatiques selon le System Reference Document
/// </summary>
public class DndSpellcastingRulesTests
{
    [Theory]
    [InlineData("magicien", SpellcastingAbility.Intelligence)]
    [InlineData("wizard", SpellcastingAbility.Intelligence)]
    [InlineData("occultiste", SpellcastingAbility.Intelligence)]
    [InlineData("warlock", SpellcastingAbility.Intelligence)]
    [InlineData("clerc", SpellcastingAbility.Wisdom)]
    [InlineData("cleric", SpellcastingAbility.Wisdom)]
    [InlineData("druide", SpellcastingAbility.Wisdom)]
    [InlineData("druid", SpellcastingAbility.Wisdom)]
    [InlineData("rôdeur", SpellcastingAbility.Wisdom)]
    [InlineData("ranger", SpellcastingAbility.Wisdom)]
    [InlineData("barde", SpellcastingAbility.Charisma)]
    [InlineData("bard", SpellcastingAbility.Charisma)]
    [InlineData("ensorceleur", SpellcastingAbility.Charisma)]
    [InlineData("sorcerer", SpellcastingAbility.Charisma)]
    [InlineData("paladin", SpellcastingAbility.Charisma)]
    public void GetSpellcastingAbility_ReturnsCorrectAbility(string characterClass, SpellcastingAbility expected)
    {
        // Act
        var result = DndSpellcastingRules.GetSpellcastingAbility(characterClass);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData("guerrier")]
    [InlineData("fighter")]
    [InlineData("roublard")]
    [InlineData("rogue")]
    [InlineData("barbare")]
    [InlineData("barbarian")]
    public void GetSpellcastingAbility_NonSpellcasterClass_ThrowsException(string characterClass)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => DndSpellcastingRules.GetSpellcastingAbility(characterClass));
    }
    
    [Theory]
    [InlineData(8, -1)]   // Score 8 → Modificateur -1
    [InlineData(10, 0)]   // Score 10 → Modificateur 0
    [InlineData(12, 1)]   // Score 12 → Modificateur +1
    [InlineData(14, 2)]   // Score 14 → Modificateur +2
    [InlineData(16, 3)]   // Score 16 → Modificateur +3
    [InlineData(18, 4)]   // Score 18 → Modificateur +4
    [InlineData(20, 5)]   // Score 20 → Modificateur +5
    public void CalculateAbilityModifier_ReturnsCorrectModifier(int abilityScore, int expectedModifier)
    {
        // Act
        var result = DndSpellcastingRules.CalculateAbilityModifier(abilityScore);
        
        // Assert
        Assert.Equal(expectedModifier, result);
    }
    
    [Theory]
    [InlineData(1, 2)]    // Niveau 1-4 → +2
    [InlineData(4, 2)]
    [InlineData(5, 3)]    // Niveau 5-8 → +3
    [InlineData(8, 3)]
    [InlineData(9, 4)]    // Niveau 9-12 → +4
    [InlineData(12, 4)]
    [InlineData(13, 5)]   // Niveau 13-16 → +5
    [InlineData(16, 5)]
    [InlineData(17, 6)]   // Niveau 17-20 → +6
    [InlineData(20, 6)]
    public void CalculateProficiencyBonus_ReturnsCorrectBonus(int characterLevel, int expectedBonus)
    {
        // Act
        var result = DndSpellcastingRules.CalculateProficiencyBonus(characterLevel);
        
        // Assert
        Assert.Equal(expectedBonus, result);
    }
    
    [Fact]
    public void CalculateSpellAttackBonus_MageLevel5Intelligence18_Returns7()
    {
        // Arrange
        int intelligence = 18; // Modificateur +4
        int proficiencyBonus = 3; // Niveau 5
        
        // Act
        var result = DndSpellcastingRules.CalculateSpellAttackBonus(intelligence, proficiencyBonus);
        
        // Assert
        Assert.Equal(7, result); // +4 (Int) + +3 (maîtrise) = +7
    }
    
    [Fact]
    public void CalculateSpellSaveDC_MageLevel5Intelligence18_Returns15()
    {
        // Arrange
        int intelligence = 18; // Modificateur +4
        int proficiencyBonus = 3; // Niveau 5
        
        // Act
        var result = DndSpellcastingRules.CalculateSpellSaveDC(intelligence, proficiencyBonus);
        
        // Assert
        Assert.Equal(15, result); // 8 + 4 (Int) + 3 (maîtrise) = 15
    }
    
    [Fact]
    public void CalculateSpellSaveDC_PaladinLevel3Charisma14_Returns12()
    {
        // Arrange
        int charisma = 14; // Modificateur +2
        int proficiencyBonus = 2; // Niveau 3
        
        // Act
        var result = DndSpellcastingRules.CalculateSpellSaveDC(charisma, proficiencyBonus);
        
        // Assert
        Assert.Equal(12, result); // 8 + 2 (Cha) + 2 (maîtrise) = 12
    }
    
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(4, 2)]
    [InlineData(5, 3)]
    [InlineData(6, 3)]
    [InlineData(9, 5)]
    [InlineData(17, 9)]
    [InlineData(20, 9)]
    public void GetMaxSpellLevel_ReturnsCorrectMaxLevel(int characterLevel, int expectedMaxLevel)
    {
        // Act
        var result = DndSpellcastingRules.GetMaxSpellLevel(characterLevel);
        
        // Assert
        Assert.Equal(expectedMaxLevel, result);
    }
    
    [Fact]
    public void CalculateSpellcastingStats_MageLevel5Intelligence18_ReturnsCorrectStats()
    {
        // Arrange
        string characterClass = "magicien";
        int characterLevel = 5;
        int intelligence = 18;
        
        // Act
        var result = DndSpellcastingRules.CalculateSpellcastingStats(characterClass, characterLevel, intelligence);
        
        // Assert
        Assert.Equal(SpellcastingAbility.Intelligence, result.SpellcastingAbility);
        Assert.Equal(4, result.AbilityModifier); // (18-10)/2 = 4
        Assert.Equal(3, result.ProficiencyBonus); // Niveau 5 = +3
        Assert.Equal(7, result.SpellAttackBonus); // 4 + 3 = 7
        Assert.Equal(15, result.SpellSaveDC); // 8 + 4 + 3 = 15
        Assert.Equal(3, result.MaxSpellLevel); // Niveau 5 = sorts niveau 3 max
    }
    
    [Theory]
    [InlineData("magicien", true)]
    [InlineData("clerc", true)]
    [InlineData("barde", true)]
    [InlineData("guerrier", false)]
    [InlineData("roublard", false)]
    [InlineData("", false)]
    public void IsSpellcaster_ReturnsCorrectValue(string characterClass, bool expectedResult)
    {
        // Act
        var result = DndSpellcastingRules.IsSpellcaster(characterClass);
        
        // Assert
        Assert.Equal(expectedResult, result);
    }
}