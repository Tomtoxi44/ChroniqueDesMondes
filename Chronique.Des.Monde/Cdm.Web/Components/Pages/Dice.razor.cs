using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Cdm.Web.Components.Pages;

public partial class Dice : ComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private int diceCount = 1;
    private int selectedDiceType = 20;
    private int modifier = 0;
    private bool isRolling = false;
    
    private DiceResult? lastResult;
    private List<DiceResult> rollHistory = new();
    
    private readonly List<SelectOption> diceTypes = new()
    {
        new(4, "d4 - Tétraèdre"),
        new(6, "d6 - Cube"),
        new(8, "d8 - Octaèdre"),
        new(10, "d10 - Décaèdre"),
        new(12, "d12 - Dodécaèdre"),
        new(20, "d20 - Icosaèdre"),
        new(100, "d100 - Percentile")
    };

    private async Task RollDice()
    {
        isRolling = true;
        StateHasChanged();

        // Simulation du temps de lancer
        await Task.Delay(500);

        var random = new Random();
        var diceResults = new List<int>();
        
        for (int i = 0; i < diceCount; i++)
        {
            diceResults.Add(random.Next(1, selectedDiceType + 1));
        }
        
        var total = diceResults.Sum() + modifier;
        var formula = $"{diceCount}d{selectedDiceType}";
        if (modifier != 0)
        {
            formula += modifier > 0 ? $"+{modifier}" : $"{modifier}";
        }
        
        lastResult = new DiceResult
        {
            Formula = formula,
            DiceResults = diceResults,
            Modifier = modifier,
            Total = total,
            Timestamp = DateTime.Now
        };
        
        rollHistory.Insert(0, lastResult);
        
        isRolling = false;
        StateHasChanged();

        // Animation du résultat
        await JSRuntime.InvokeVoidAsync("animateDiceResult");
    }

    private void ClearHistory()
    {
        rollHistory.Clear();
        lastResult = null;
    }

    private string GetResultClass(int total)
    {
        if (selectedDiceType == 20 && diceCount == 1)
        {
            return total switch
            {
                20 => "critical-success",
                1 => "critical-failure",
                >= 15 => "high-roll",
                <= 5 => "low-roll",
                _ => "normal-roll"
            };
        }
        
        return "normal-roll";
    }

    public class DiceResult
    {
        public string Formula { get; set; } = "";
        public List<int> DiceResults { get; set; } = new();
        public int Modifier { get; set; }
        public int Total { get; set; }
        public DateTime Timestamp { get; set; }
    }
    
    public record SelectOption(int Value, string Text);
}