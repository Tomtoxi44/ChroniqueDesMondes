using Microsoft.AspNetCore.Components;

namespace Cdm.Web.Components.Pages;

public partial class Spells : ComponentBase
{
    private string searchTerm = "";
    private string selectedSchool = "";
    private List<Spell> allSpells = new();
    private List<Spell> filteredSpells = new();

    protected override void OnInitialized()
    {
        LoadSpells();
        filteredSpells = allSpells;
    }

    protected override void OnParametersSet()
    {
        FilterSpells();
    }

    private void LoadSpells()
    {
        allSpells = new List<Spell>
        {
            new() { Name = "Boule de Feu", School = "Évocation", Level = 3, Icon = "🔥", CastingTime = "1 action", Range = "45 m", Duration = "Instantané", 
                Description = "Une boule de feu éclate à un point que vous choisissez à portée, infligeant des dégâts de feu dans une sphère.", 
                Components = new[] { "V", "S", "M" } },
            
            new() { Name = "Charme-Personne", School = "Enchantement", Level = 1, Icon = "💫", CastingTime = "1 action", Range = "9 m", Duration = "1 heure", 
                Description = "Une créature humanoïde que vous pouvez voir à portée doit effectuer un jet de sauvegarde de Sagesse.", 
                Components = new[] { "V", "S" } },
            
            new() { Name = "Invisibilité", School = "Illusion", Level = 2, Icon = "👻", CastingTime = "1 action", Range = "Contact", Duration = "1 heure", 
                Description = "Une créature que vous touchez devient invisible jusqu'à la fin du sort.", 
                Components = new[] { "V", "S", "M" } },
            
            new() { Name = "Rayon Ardent", School = "Évocation", Level = 2, Icon = "☀️", CastingTime = "1 action", Range = "36 m", Duration = "Instantané", 
                Description = "Vous créez trois rayons de feu et les lancez sur des cibles à portée.", 
                Components = new[] { "V", "S" } },
            
            new() { Name = "Fléau", School = "Nécromancie", Level = 1, Icon = "☠️", CastingTime = "1 action", Range = "9 m", Duration = "1 minute", 
                Description = "Jusqu'à trois créatures de votre choix que vous pouvez voir à portée doivent effectuer un jet de sauvegarde.", 
                Components = new[] { "V", "S", "M" } },
            
            new() { Name = "Hâte", School = "Transmutation", Level = 3, Icon = "⚡", CastingTime = "1 action", Range = "9 m", Duration = "1 minute", 
                Description = "Choisissez une créature consentante que vous pouvez voir à portée. Jusqu'à la fin du sort, la vitesse de la cible est doublée.", 
                Components = new[] { "V", "S", "M" } },
            
            new() { Name = "Détection de la Magie", School = "Divination", Level = 1, Icon = "🔮", CastingTime = "1 action", Range = "Personnelle", Duration = "10 minutes", 
                Description = "Pendant la durée du sort, vous percevez la présence de magie dans un rayon de 9 mètres autour de vous.", 
                Components = new[] { "V", "S" } },
            
            new() { Name = "Éclair", School = "Évocation", Level = 3, Icon = "⚡", CastingTime = "1 action", Range = "Personnelle", Duration = "Instantané", 
                Description = "Un éclair jaillit de vous dans une ligne de 30 mètres de long et 1,5 mètre de large dans une direction que vous choisissez.", 
                Components = new[] { "V", "S", "M" } },
            
            new() { Name = "Sommeil", School = "Enchantement", Level = 1, Icon = "💤", CastingTime = "1 action", Range = "27 m", Duration = "1 minute", 
                Description = "Ce sort envoie des créatures dans un sommeil magique. Lancez 5d8 ; le total correspond aux points de vie des créatures affectées.", 
                Components = new[] { "V", "S", "M" } },
            
            new() { Name = "Vol", School = "Transmutation", Level = 3, Icon = "🕊️", CastingTime = "1 action", Range = "Contact", Duration = "10 minutes", 
                Description = "Vous touchez une créature consentante. La cible gagne une vitesse de vol de 18 mètres pendant la durée.", 
                Components = new[] { "V", "S", "M" } },
            
            new() { Name = "Confusion", School = "Enchantement", Level = 4, Icon = "🌀", CastingTime = "1 action", Range = "27 m", Duration = "1 minute", 
                Description = "Ce sort agresse et déforme l'esprit des créatures, générant des illusions et provoquant des actions incontrôlées.", 
                Components = new[] { "V", "S", "M" } },
            
            new() { Name = "Image Miroir", School = "Illusion", Level = 2, Icon = "🪞", CastingTime = "1 action", Range = "Personnelle", Duration = "1 minute", 
                Description = "Trois duplicatas illusoires de votre personne apparaissent dans votre espace.", 
                Components = new[] { "V", "S" } }
        };
    }

    private void FilterSpells()
    {
        filteredSpells = allSpells.Where(spell =>
            (string.IsNullOrEmpty(searchTerm) || spell.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(selectedSchool) || spell.School == selectedSchool)
        ).ToList();
        
        StateHasChanged();
    }

    private string GetSpellRarityClass(int level)
    {
        return level switch
        {
            0 => "cantrip",
            1 => "common",
            2 => "common",
            3 => "uncommon",
            4 => "uncommon",
            5 => "rare",
            6 => "rare",
            7 => "epic",
            8 => "epic",
            9 => "legendary",
            _ => "common"
        };
    }

    public class Spell
    {
        public string Name { get; set; } = "";
        public string School { get; set; } = "";
        public int Level { get; set; }
        public string Icon { get; set; } = "";
        public string CastingTime { get; set; } = "";
        public string Range { get; set; } = "";
        public string Duration { get; set; } = "";
        public string Description { get; set; } = "";
        public string[] Components { get; set; } = Array.Empty<string>();
    }
}