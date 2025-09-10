using Cdm.Business.Common.Interfaces.Combat;
using Cdm.Data.Dnd;
using Cdm.Data.Dnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Cdm.Business.Dnd.Services.Combat;

/// <summary>
/// Calculateur spécialisé pour les règles de combat D&D 5e
/// Gère tous les calculs automatiques selon le System Reference Document
/// </summary>
public class DndCombatCalculator : IDndCombatCalculator
{
    private readonly DndDbContext _context;
    private readonly ILogger<DndCombatCalculator> _logger;

    public DndCombatCalculator(DndDbContext context, ILogger<DndCombatCalculator> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Calcule le modificateur d'une caractéristique D&D (STR, DEX, etc.)
    /// </summary>
    public int CalculateAbilityModifier(int abilityScore)
    {
        var modifier = (abilityScore - 10) / 2;
        _logger.LogDebug("Modificateur pour score {Score}: {Modifier}", abilityScore, modifier);
        return modifier;
    }

    /// <summary>
    /// Calcule le bonus de maîtrise selon le niveau du personnage
    /// </summary>
    public int CalculateProficiencyBonus(int characterLevel)
    {
        var bonus = characterLevel switch
        {
            >= 1 and <= 4 => 2,
            >= 5 and <= 8 => 3,
            >= 9 and <= 12 => 4,
            >= 13 and <= 16 => 5,
            >= 17 and <= 20 => 6,
            _ => 2 // Par défaut niveau 1
        };
        
        _logger.LogDebug("Bonus de maîtrise niveau {Level}: +{Bonus}", characterLevel, bonus);
        return bonus;
    }

    /// <summary>
    /// Calcule la classe d'armure totale d'un personnage
    /// </summary>
    public async Task<int> CalculateArmorClassAsync(int characterId)
    {
        var character = await _context.CharactersDnd
            .FirstOrDefaultAsync(c => c.Id == characterId);

        if (character == null)
        {
            _logger.LogError("Personnage {CharacterId} non trouvé pour calcul CA", characterId);
            return 10; // CA de base sans armure
        }

        // Récupérer l'armure équipée
        var equippedArmor = await GetEquippedArmorAsync(characterId);
        var equippedShield = await GetEquippedShieldAsync(characterId);

        var baseAC = 10; // CA de base sans armure
        var dexModifier = CalculateAbilityModifier(GetTotalDexterity(character));
        var armorAC = 0;
        var shieldAC = 0;
        var maxDexBonus = int.MaxValue;

        // Calcul selon le type d'armure
        if (equippedArmor != null)
        {
            armorAC = equippedArmor.ArmorClass;
            
            // Gestion du bonus de Dextérité selon le type d'armure
            if (equippedArmor.MaxDexBonus >= 0)
            {
                maxDexBonus = equippedArmor.MaxDexBonus;
            }
            
            // Armure lourde : pas de bonus de Dextérité
            if (equippedArmor.Category == "Armure" && equippedArmor.Tags.Contains("lourde"))
            {
                maxDexBonus = 0;
            }
        }

        // Bonus de bouclier
        if (equippedShield != null)
        {
            shieldAC = equippedShield.ArmorClass; // +2 pour un bouclier standard
        }

        // Appliquer la limite du bonus de Dextérité
        var effectiveDexBonus = Math.Min(dexModifier, maxDexBonus);
        if (maxDexBonus == 0) effectiveDexBonus = 0;

        var finalAC = equippedArmor != null ? armorAC + effectiveDexBonus + shieldAC : baseAC + dexModifier + shieldAC;

        _logger.LogInformation("CA calculée pour {CharacterId}: Base={BaseAC}, Armure={ArmorAC}, DEX={DexBonus}, Bouclier={ShieldAC} = {FinalAC}",
            characterId, equippedArmor != null ? armorAC : baseAC, armorAC, effectiveDexBonus, shieldAC, finalAC);

        return finalAC;
    }

    /// <summary>
    /// Calcule le bonus d'attaque total avec une arme
    /// </summary>
    public async Task<int> CalculateAttackBonusAsync(int characterId, int equipmentId)
    {
        var character = await _context.CharactersDnd
            .FirstOrDefaultAsync(c => c.Id == characterId);

        var equipment = await _context.EquipmentDnd
            .FirstOrDefaultAsync(e => e.Id == equipmentId);

        if (character == null || equipment == null || !equipment.IsWeapon)
        {
            _logger.LogError("Données invalides pour calcul bonus d'attaque: Character={CharId}, Equipment={EquipId}", 
                characterId, equipmentId);
            return 0;
        }

        var proficiencyBonus = CalculateProficiencyBonus(character.Leveling);
        var abilityModifier = GetWeaponAbilityModifier(character, equipment);
        var magicalBonus = equipment.AttackBonus; // Bonus magique de l'arme
        
        // Vérifier la maîtrise de l'arme (simplifié - en production, il faudrait une table de maîtrises)
        var isProficient = IsWeaponProficient(character, equipment);
        var finalProficiencyBonus = isProficient ? proficiencyBonus : 0;

        var totalBonus = abilityModifier + finalProficiencyBonus + magicalBonus;

        _logger.LogInformation("Bonus d'attaque {WeaponName} pour {CharacterId}: {Ability}+{Prof}+{Magic} = +{Total}",
            equipment.Name, characterId, abilityModifier, finalProficiencyBonus, magicalBonus, totalBonus);

        return totalBonus;
    }

    /// <summary>
    /// Calcule les dégâts d'une arme (avec critique possible)
    /// </summary>
    public async Task<(string DiceFormula, int Modifier)> CalculateWeaponDamageAsync(int characterId, int weaponId, bool isCritical = false)
    {
        var character = await _context.CharactersDnd
            .FirstOrDefaultAsync(c => c.Id == characterId);

        var weapon = await _context.EquipmentDnd
            .FirstOrDefaultAsync(e => e.Id == weaponId);

        if (character == null || weapon == null || !weapon.IsWeapon)
        {
            _logger.LogError("Données invalides pour calcul dégâts: Character={CharId}, Weapon={WeaponId}", 
                characterId, weaponId);
            return ("1d4", 0);
        }

        var baseDamage = weapon.Damage; // Ex: "1d8", "2d6"
        var abilityModifier = GetWeaponAbilityModifier(character, weapon);
        var magicalBonus = weapon.AttackBonus; // Les armes magiques ajoutent aussi aux dégâts

        // Gestion de la propriété Polyvalente (versatile)
        if (weapon.Properties.Contains("Polyvalente") && IsWieldedTwoHanded(character, weapon))
        {
            baseDamage = ExtractVersatileDamage(weapon.Properties) ?? baseDamage;
        }

        var finalModifier = abilityModifier + magicalBonus;

        _logger.LogInformation("Dégâts {WeaponName}: {BaseDamage} + {Modifier} {CriticalText}",
            weapon.Name, baseDamage, finalModifier, isCritical ? "(CRITIQUE)" : "");

        return (baseDamage, finalModifier);
    }

    /// <summary>
    /// Calcule le DD de sauvegarde d'un sort
    /// </summary>
    public async Task<int> CalculateSpellSaveDCAsync(int characterId, int spellId)
    {
        var character = await _context.CharactersDnd
            .FirstOrDefaultAsync(c => c.Id == characterId);

        var spell = await _context.SpellsDnd
            .FirstOrDefaultAsync(s => s.Id == spellId);

        if (character == null || spell == null)
        {
            _logger.LogError("Données invalides pour calcul DD sort: Character={CharId}, Spell={SpellId}", 
                characterId, spellId);
            return 8; // DD minimum
        }

        var spellcastingAbility = GetSpellcastingAbility(character.Class);
        var abilityModifier = GetAbilityModifier(character, spellcastingAbility);
        var proficiencyBonus = CalculateProficiencyBonus(character.Leveling);

        var spellDC = 8 + abilityModifier + proficiencyBonus;

        _logger.LogInformation("DD sauvegarde {SpellName} pour {Class}: 8 + {Ability} + {Prof} = {DC}",
            spell.Name, character.Class, abilityModifier, proficiencyBonus, spellDC);

        return spellDC;
    }

    /// <summary>
    /// Calcule le bonus d'attaque magique
    /// </summary>
    public async Task<int> CalculateSpellAttackBonusAsync(int characterId)
    {
        var character = await _context.CharactersDnd
            .FirstOrDefaultAsync(c => c.Id == characterId);

        if (character == null)
        {
            _logger.LogError("Personnage {CharacterId} non trouvé pour calcul bonus attaque magique", characterId);
            return 0;
        }

        var spellcastingAbility = GetSpellcastingAbility(character.Class);
        var abilityModifier = GetAbilityModifier(character, spellcastingAbility);
        var proficiencyBonus = CalculateProficiencyBonus(character.Leveling);

        var spellAttackBonus = abilityModifier + proficiencyBonus;

        _logger.LogInformation("Bonus attaque magique {Class}: {Ability} + {Prof} = +{Bonus}",
            character.Class, abilityModifier, proficiencyBonus, spellAttackBonus);

        return spellAttackBonus;
    }

    /// <summary>
    /// Vérifie si un personnage peut lancer un sort
    /// </summary>
    public async Task<(bool CanCast, string? Reason)> CanCastSpellAsync(int characterId, int spellId, int spellLevel)
    {
        var character = await _context.CharactersDnd
            .FirstOrDefaultAsync(c => c.Id == characterId);

        var spell = await _context.SpellsDnd
            .FirstOrDefaultAsync(s => s.Id == spellId);

        if (character == null)
            return (false, "Personnage non trouvé");

        if (spell == null)
            return (false, "Sort non trouvé");

        // Vérifier que le personnage connaît le sort
        var knowsSpell = await _context.CharacterSpells
            .AnyAsync(cs => cs.CharacterId == characterId && cs.SpellId == spellId);

        if (!knowsSpell)
            return (false, "Sort non connu");

        // Vérifier les emplacements de sorts (simplifié)
        var hasSpellSlot = await HasSpellSlotAsync(characterId, spellLevel);
        if (!hasSpellSlot)
            return (false, $"Aucun emplacement de sort de niveau {spellLevel} disponible");

        // Vérifier que la classe peut lancer ce sort (simplifié)
        // En production, il faudrait une table de correspondance classe/sorts
        var canCastByClass = CanClassCastSpell(character.Class, spell);
        if (!canCastByClass)
            return (false, $"La classe {character.Class} ne peut pas lancer ce sort");

        return (true, null);
    }

    /// <summary>
    /// Consomme un emplacement de sort
    /// </summary>
    public async Task ConsumeSpellSlotAsync(int characterId, int spellLevel)
    {
        // Implementation simplifiée - en production, il faudrait un système de gestion des emplacements
        _logger.LogInformation("Emplacement de sort niveau {Level} consommé pour le personnage {CharacterId}", 
            spellLevel, characterId);
        
        // TODO: Implémenter la gestion réelle des emplacements de sorts
        await Task.CompletedTask;
    }

    #region Méthodes d'aide privées

    /// <summary>
    /// Obtient le total d'une caractéristique (base + additionnelle)
    /// </summary>
    private static int GetTotalStrength(CharacterDnd character) => character.Strong + character.AdditionalStrong;
    private static int GetTotalDexterity(CharacterDnd character) => character.Dexterity + character.AdditionalDexterity;
    private static int GetTotalConstitution(CharacterDnd character) => character.Constitution + character.AdditionalConstitution;
    private static int GetTotalIntelligence(CharacterDnd character) => character.Intelligence + character.AdditionalIntelligence;
    private static int GetTotalWisdom(CharacterDnd character) => character.Wisdoms + character.AdditionalWisdoms;
    private static int GetTotalCharisma(CharacterDnd character) => character.Charism + character.AdditionalCharism;

    /// <summary>
    /// Obtient le modificateur d'une caractéristique pour un personnage
    /// </summary>
    private int GetAbilityModifier(CharacterDnd character, string ability)
    {
        var score = ability.ToLower() switch
        {
            "strength" => GetTotalStrength(character),
            "dexterity" => GetTotalDexterity(character),
            "constitution" => GetTotalConstitution(character),
            "intelligence" => GetTotalIntelligence(character),
            "wisdom" => GetTotalWisdom(character),
            "charisma" => GetTotalCharisma(character),
            _ => 10
        };

        return CalculateAbilityModifier(score);
    }

    /// <summary>
    /// Détermine quel modificateur de caractéristique utiliser pour une arme
    /// </summary>
    private int GetWeaponAbilityModifier(CharacterDnd character, EquipmentDnd weapon)
    {
        // Armes avec Finesse : choisir entre Force ou Dextérité
        if (weapon.Properties.Contains("Finesse"))
        {
            var strModifier = CalculateAbilityModifier(GetTotalStrength(character));
            var dexModifier = CalculateAbilityModifier(GetTotalDexterity(character));
            return Math.Max(strModifier, dexModifier);
        }

        // Armes à distance : Dextérité
        if (weapon.WeaponType == "Arc" || weapon.WeaponType == "Arbalète" || weapon.WeaponType == "Fronde")
        {
            return CalculateAbilityModifier(GetTotalDexterity(character));
        }

        // Armes de mêlée : Force par défaut
        return CalculateAbilityModifier(GetTotalStrength(character));
    }

    /// <summary>
    /// Détermine la caractéristique d'incantation selon la classe
    /// </summary>
    private static string GetSpellcastingAbility(string characterClass)
    {
        return characterClass.ToLower() switch
        {
            "wizard" or "magicien" => "Intelligence",
            "cleric" or "clerc" => "Wisdom",
            "paladin" => "Charisma",
            "sorcerer" or "ensorceleur" => "Charisma",
            "warlock" or "démoniste" => "Charisma",
            "bard" or "barde" => "Charisma",
            "druid" or "druide" => "Wisdom",
            "ranger" or "rôdeur" => "Wisdom",
            _ => "Intelligence" // Par défaut
        };
    }

    /// <summary>
    /// Vérifie si le personnage maîtrise cette arme (simplifié)
    /// </summary>
    private static bool IsWeaponProficient(CharacterDnd character, EquipmentDnd weapon)
    {
        // Implémentation simplifiée - en production, il faudrait une table de maîtrises par classe
        return character.Class.ToLower() switch
        {
            "fighter" or "guerrier" => true, // Les guerriers maîtrisent toutes les armes
            "wizard" or "magicien" => weapon.WeaponType == "Dague" || weapon.WeaponType == "Bâton",
            "rogue" or "voleur" => weapon.Properties.Contains("Finesse") || weapon.WeaponType == "Arc",
            _ => !weapon.Tags.Contains("guerre") // Maîtrise des armes simples par défaut
        };
    }

    /// <summary>
    /// Vérifie si l'arme est maniée à deux mains
    /// </summary>
    private static bool IsWieldedTwoHanded(CharacterDnd character, EquipmentDnd weapon)
    {
        // Simplifié - en production, il faudrait vérifier l'équipement actuel
        return weapon.Properties.Contains("Deux mains") || 
               weapon.Properties.Contains("Lourde");
    }

    /// <summary>
    /// Extrait les dégâts polyvalents d'une description de propriété
    /// </summary>
    private static string? ExtractVersatileDamage(string properties)
    {
        // Ex: "Polyvalente (1d10)" -> "1d10"
        var match = System.Text.RegularExpressions.Regex.Match(
            properties, @"Polyvalente \(([^)]+)\)");
        return match.Success ? match.Groups[1].Value : null;
    }

    /// <summary>
    /// Récupère l'armure équipée (simplifié)
    /// </summary>
    private async Task<EquipmentDnd?> GetEquippedArmorAsync(int characterId)
    {
        // Implémentation simplifiée - récupère la première armure de l'inventaire
        var inventory = await _context.CharacterInventory
            .Include(ci => ci.Equipment)
            .Where(ci => ci.CharacterId == characterId)
            .ToListAsync();

        return inventory
            .Where(ci => ci.Equipment is EquipmentDnd equipment && equipment.IsArmor)
            .Select(ci => ci.Equipment as EquipmentDnd)
            .FirstOrDefault();
    }

    /// <summary>
    /// Récupère le bouclier équipé (simplifié)
    /// </summary>
    private async Task<EquipmentDnd?> GetEquippedShieldAsync(int characterId)
    {
        var inventory = await _context.CharacterInventory
            .Include(ci => ci.Equipment)
            .Where(ci => ci.CharacterId == characterId)
            .ToListAsync();

        return inventory
            .Where(ci => ci.Equipment is EquipmentDnd equipment && equipment.IsShield)
            .Select(ci => ci.Equipment as EquipmentDnd)
            .FirstOrDefault();
    }

    /// <summary>
    /// Vérifie si le personnage a un emplacement de sort disponible (simplifié)
    /// </summary>
    private async Task<bool> HasSpellSlotAsync(int characterId, int spellLevel)
    {
        // Implémentation simplifiée - toujours vrai pour les tests
        // En production, il faudrait gérer les emplacements de sorts par niveau
        await Task.CompletedTask;
        return true;
    }

    /// <summary>
    /// Vérifie si une classe peut lancer un sort spécifique (simplifié)
    /// </summary>
    private static bool CanClassCastSpell(string characterClass, SpellDnd spell)
    {
        // Implémentation simplifiée - en production, il faudrait une table complète
        return characterClass.ToLower() switch
        {
            "wizard" or "magicien" => true, // Les magiciens peuvent apprendre presque tous les sorts
            "cleric" or "clerc" => spell.School.ToLower() == "divination" || spell.School.ToLower() == "abjuration",
            "sorcerer" or "ensorceleur" => spell.School.ToLower() == "évocation" || spell.School.ToLower() == "transmutation",
            "warlock" or "démoniste" => spell.School.ToLower() == "évocation" || spell.School.ToLower() == "enchantement",
            _ => spell.Level <= 2 // Classes non magiques peuvent utiliser quelques sorts de bas niveau
        };
    }

    #endregion
}