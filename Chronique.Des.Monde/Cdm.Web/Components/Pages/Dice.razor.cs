using Microsoft.AspNetCore.Components;

namespace Cdm.Web.Components.Pages;

public partial class Dice : ComponentBase
{
    private readonly int[] availableDice = { 4, 6, 8, 10, 12, 20, 100 };
    private readonly HashSet<int> selectedDice = new();
    private int diceCount = 1;
    private int modifier = 0;
    private DiceRoll? lastRoll;
    private readonly List<DiceRoll> rollHistory = new();
    private readonly Random random = new();

    private void ToggleDice(int die)
    {
        if (selectedDice.Contains(die))
        {
            selectedDice.Remove(die);
        }
        else
        {
            selectedDice.Clear();
            selectedDice.Add(die);
        }
    }

    private void RollDice()
    {
        if (!selectedDice.Any()) return;

        var die = selectedDice.First();
        var rolls = new List<int>();
        
        for (int i = 0; i < diceCount; i++)
        {
            rolls.Add(random.Next(1, die + 1));
        }

        var total = rolls.Sum() + modifier;
        var formula = $"{diceCount}d{die}";
        if (modifier != 0)
        {
            formula += modifier >= 0 ? $" + {modifier}" : $" - {Math.Abs(modifier)}";
        }

        lastRoll = new DiceRoll
        {
            Formula = formula,
            IndividualRolls = rolls,
            Modifier = modifier,
            Total = total,
            Time = DateTime.Now,
            IsCritical = die == 20 && rolls.Any(r => r == 20),
            IsFumble = die == 20 && rolls.Any(r => r == 1)
        };

        rollHistory.Insert(0, lastRoll);
        StateHasChanged();
    }

    private void QuickRoll(int die, int count, int mod)
    {
        selectedDice.Clear();
        selectedDice.Add(die);
        diceCount = count;
        modifier = mod;
        RollDice();
    }

    private void ClearHistory()
    {
        rollHistory.Clear();
        StateHasChanged();
    }

    private string GetDiceIcon(int die)
    {
        return die switch
        {
            4 => "🔺",
            6 => "⚀",
            8 => "🔶",
            10 => "🔟",
            12 => "🟢",
            20 => "🎲",
            100 => "💯",
            _ => "🎲"
        };
    }

    private string GetHistoryClass(DiceRoll roll)
    {
        var classes = new List<string> { "history-item" };
        
        if (roll.IsCritical) classes.Add("critical");
        if (roll.IsFumble) classes.Add("fumble");
        
        return string.Join(" ", classes);
    }

    private int GetAbilityModifier()
    {
        // Simuler un modificateur de caractéristique typique
        return random.Next(0, 6);
    }

    private int GetAttackBonus()
    {
        // Simuler un bonus d'attaque typique
        return random.Next(2, 8);
    }

    public class DiceRoll
    {
        public string Formula { get; set; } = "";
        public List<int> IndividualRolls { get; set; } = new();
        public int Modifier { get; set; }
        public int Total { get; set; }
        public DateTime Time { get; set; }
        public bool IsCritical { get; set; }
        public bool IsFumble { get; set; }
    }
}