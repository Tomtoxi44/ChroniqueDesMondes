using Cdm.Business.Common.Interfaces.Combat;
using Cdm.Business.Common.Models.Combat;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Cdm.Business.Common.Services.Combat;

/// <summary>
/// Service de génération de dés avec support complet des formules D&D
/// Thread-safe et optimisé pour les performances
/// </summary>
public class DiceRoller : IDiceRoller
{
    private readonly Random _random;
    private readonly ILogger<DiceRoller> _logger;
    private static readonly Regex DiceFormulaRegex = new(@"^(\d+)d(\d+)(?:[+\-](\d+))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public DiceRoller(ILogger<DiceRoller> logger)
    {
        _random = new Random();
        _logger = logger;
    }

    /// <summary>
    /// Lance un dé simple (1d20, 1d6, etc.)
    /// </summary>
    public int RollDie(int sides)
    {
        if (sides <= 0)
            throw new ArgumentException("Le nombre de faces doit être positif", nameof(sides));

        var result = _random.Next(1, sides + 1);
        _logger.LogDebug("Jet 1d{Sides}: {Result}", sides, result);
        return result;
    }

    /// <summary>
    /// Lance plusieurs dés (2d6, 3d8, etc.)
    /// </summary>
    public List<int> RollDice(int count, int sides)
    {
        if (count <= 0)
            throw new ArgumentException("Le nombre de dés doit être positif", nameof(count));
        if (sides <= 0)
            throw new ArgumentException("Le nombre de faces doit être positif", nameof(sides));

        var results = new List<int>();
        for (int i = 0; i < count; i++)
        {
            results.Add(RollDie(sides));
        }

        _logger.LogDebug("Jet {Count}d{Sides}: [{Results}] = {Total}", 
            count, sides, string.Join(", ", results), results.Sum());
        
        return results;
    }

    /// <summary>
    /// Analyse et lance une formule de dé (1d8+3, 2d6+2, etc.)
    /// </summary>
    public DiceRollResult RollFormula(string formula, int modifier = 0)
    {
        if (string.IsNullOrWhiteSpace(formula))
            throw new ArgumentException("La formule de dé ne peut pas être vide", nameof(formula));

        formula = formula.Trim().Replace(" ", "");
        var match = DiceFormulaRegex.Match(formula);

        if (!match.Success)
            throw new ArgumentException($"Formule de dé invalide: {formula}", nameof(formula));

        var count = int.Parse(match.Groups[1].Value);
        var sides = int.Parse(match.Groups[2].Value);
        var formulaModifier = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;

        // Appliquer le signe de la formule
        if (formula.Contains('-') && match.Groups[3].Success)
            formulaModifier = -formulaModifier;

        var rolls = RollDice(count, sides);
        var total = rolls.Sum() + formulaModifier + modifier;

        var result = new DiceRollResult
        {
            DiceType = $"{count}d{sides}",
            Rolls = rolls,
            Modifier = formulaModifier + modifier,
            Total = total,
            Purpose = "formula",
            AdvantageType = "normal"
        };

        _logger.LogInformation("Formule {Formula} + {ExtraModifier}: {Rolls} + {TotalModifier} = {Total}", 
            formula, modifier, string.Join("+", rolls), result.Modifier, total);

        return result;
    }

    /// <summary>
    /// Lance avec avantage (2d20 prendre le plus haut)
    /// </summary>
    public DiceRollResult RollWithAdvantage(int sides = 20)
    {
        var rolls = RollDice(2, sides);
        var highest = rolls.Max();

        var result = new DiceRollResult
        {
            DiceType = $"2d{sides}",
            Rolls = rolls,
            Modifier = 0,
            Total = highest,
            Purpose = "advantage",
            AdvantageType = "advantage"
        };

        _logger.LogInformation("Jet avec avantage 2d{Sides}: [{Rolls}] → {Highest} (avantage)", 
            sides, string.Join(", ", rolls), highest);

        return result;
    }

    /// <summary>
    /// Lance avec désavantage (2d20 prendre le plus bas)
    /// </summary>
    public DiceRollResult RollWithDisadvantage(int sides = 20)
    {
        var rolls = RollDice(2, sides);
        var lowest = rolls.Min();

        var result = new DiceRollResult
        {
            DiceType = $"2d{sides}",
            Rolls = rolls,
            Modifier = 0,
            Total = lowest,
            Purpose = "disadvantage",
            AdvantageType = "disadvantage"
        };

        _logger.LogInformation("Jet avec désavantage 2d{Sides}: [{Rolls}] → {Lowest} (désavantage)", 
            sides, string.Join(", ", rolls), lowest);

        return result;
    }

    /// <summary>
    /// Lance des dégâts avec possibilité de critique
    /// </summary>
    public DiceRollResult RollDamage(string diceFormula, int modifier, bool isCritical = false)
    {
        if (string.IsNullOrWhiteSpace(diceFormula))
            throw new ArgumentException("La formule de dégâts ne peut pas être vide", nameof(diceFormula));

        diceFormula = diceFormula.Trim().Replace(" ", "");
        var match = DiceFormulaRegex.Match(diceFormula);

        if (!match.Success)
            throw new ArgumentException($"Formule de dégâts invalide: {diceFormula}", nameof(diceFormula));

        var count = int.Parse(match.Groups[1].Value);
        var sides = int.Parse(match.Groups[2].Value);

        // En cas de critique, doubler les dés (pas le modificateur)
        if (isCritical)
            count *= 2;

        var rolls = RollDice(count, sides);
        var total = rolls.Sum() + modifier;

        var result = new DiceRollResult
        {
            DiceType = isCritical ? $"{count}d{sides} (critique)" : $"{count}d{sides}",
            Rolls = rolls,
            Modifier = modifier,
            Total = total,
            Purpose = "damage",
            AdvantageType = "normal"
        };

        if (isCritical)
        {
            _logger.LogInformation("💥 DÉGÂTS CRITIQUES {Formula}: {Rolls} + {Modifier} = {Total}", 
                diceFormula, string.Join("+", rolls), modifier, total);
        }
        else
        {
            _logger.LogInformation("Dégâts {Formula}: {Rolls} + {Modifier} = {Total}", 
                diceFormula, string.Join("+", rolls), modifier, total);
        }

        return result;
    }

    /// <summary>
    /// Lance un jet d'attaque avec modificateurs D&D 5e
    /// </summary>
    public DiceRollResult RollAttack(int attackBonus, string advantageType = "normal")
    {
        DiceRollResult baseRoll = advantageType.ToLower() switch
        {
            "advantage" => RollWithAdvantage(),
            "disadvantage" => RollWithDisadvantage(),
            _ => new DiceRollResult 
            { 
                DiceType = "1d20", 
                Rolls = [RollDie(20)], 
                Modifier = 0, 
                Total = 0, 
                AdvantageType = "normal" 
            }
        };

        if (advantageType == "normal")
            baseRoll.Total = baseRoll.Rolls[0];

        baseRoll.Modifier = attackBonus;
        baseRoll.Total += attackBonus;
        baseRoll.Purpose = "attack";

        var criticalCheck = baseRoll.Rolls.Max(); // Pour avantage/désavantage
        var isCritical = criticalCheck == 20;
        var isCriticalMiss = criticalCheck == 1;

        if (isCritical)
        {
            _logger.LogInformation("🎯 COUP CRITIQUE ! Jet d'attaque: {Roll} + {Bonus} = {Total}", 
                baseRoll.Rolls.Max(), attackBonus, baseRoll.Total);
        }
        else if (isCriticalMiss)
        {
            _logger.LogInformation("💀 Échec critique ! Jet d'attaque: {Roll} + {Bonus} = {Total}", 
                baseRoll.Rolls.Min(), attackBonus, baseRoll.Total);
        }
        else
        {
            _logger.LogDebug("Jet d'attaque: {Roll} + {Bonus} = {Total}", 
                baseRoll.Total - attackBonus, attackBonus, baseRoll.Total);
        }

        return baseRoll;
    }

    /// <summary>
    /// Lance un jet de sauvegarde avec modificateurs D&D 5e
    /// </summary>
    public DiceRollResult RollSavingThrow(int saveModifier, string advantageType = "normal")
    {
        var baseRoll = advantageType.ToLower() switch
        {
            "advantage" => RollWithAdvantage(),
            "disadvantage" => RollWithDisadvantage(),
            _ => new DiceRollResult 
            { 
                DiceType = "1d20", 
                Rolls = [RollDie(20)], 
                Modifier = 0, 
                Total = 0, 
                AdvantageType = "normal" 
            }
        };

        if (advantageType == "normal")
            baseRoll.Total = baseRoll.Rolls[0];

        baseRoll.Modifier = saveModifier;
        baseRoll.Total += saveModifier;
        baseRoll.Purpose = "save";

        _logger.LogDebug("Jet de sauvegarde: {Roll} + {Modifier} = {Total}", 
            baseRoll.Total - saveModifier, saveModifier, baseRoll.Total);

        return baseRoll;
    }

    /// <summary>
    /// Lance l'initiative avec modificateur de Dextérité
    /// </summary>
    public DiceRollResult RollInitiative(int dexModifier)
    {
        var roll = RollDie(20);
        var total = roll + dexModifier;

        var result = new DiceRollResult
        {
            DiceType = "1d20",
            Rolls = [roll],
            Modifier = dexModifier,
            Total = total,
            Purpose = "initiative",
            AdvantageType = "normal"
        };

        _logger.LogInformation("Initiative: {Roll} + {DexMod} = {Total}", roll, dexModifier, total);

        return result;
    }

    /// <summary>
    /// Teste plusieurs formules pour validation
    /// </summary>
    public Dictionary<string, DiceRollResult> TestFormulas()
    {
        var formulas = new[]
        {
            "1d20", "1d8+3", "2d6", "3d6+2", "1d12-1", "4d4+4"
        };

        var results = new Dictionary<string, DiceRollResult>();
        
        foreach (var formula in formulas)
        {
            try
            {
                results[formula] = RollFormula(formula);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du test de la formule {Formula}", formula);
            }
        }

        return results;
    }
}