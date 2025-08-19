using Microsoft.AspNetCore.Components;

namespace Cdm.Web.Components.Pages;

public partial class Characters : ComponentBase
{
    private List<CharacterData> characters = new();

    protected override void OnInitialized()
    {
        LoadCharacters();
    }

    private void LoadCharacters()
    {
        characters = new List<CharacterData>
        {
            new() { Name = "Gorthak le Protecteur", Race = "Nain", Class = "Guerrier", Level = 5, Icon = "🛡️",
                CurrentHP = 58, MaxHP = 65, ArmorClass = 18, Experience = 6500,
                Strength = 16, Dexterity = 10, Constitution = 15, Intelligence = 11, Wisdom = 13, Charisma = 8,
                Campaign = "Les Terres Oubliées" },

            new() { Name = "Lyralei l'Archimage", Race = "Elfe", Class = "Magicien", Level = 5, Icon = "🧙‍♀️",
                CurrentHP = 32, MaxHP = 35, ArmorClass = 12, Experience = 6100,
                Strength = 8, Dexterity = 14, Constitution = 13, Intelligence = 18, Wisdom = 15, Charisma = 12,
                Campaign = "Les Terres Oubliées" },

            new() { Name = "Aranis Chassevent", Race = "Elfe", Class = "Rôdeur", Level = 4, Icon = "🏹",
                CurrentHP = 40, MaxHP = 42, ArmorClass = 15, Experience = 4200,
                Strength = 12, Dexterity = 16, Constitution = 14, Intelligence = 13, Wisdom = 15, Charisma = 10,
                Campaign = "L'Éveil du Dragon" },

            new() { Name = "Sœur Luminara", Race = "Humain", Class = "Clerc", Level = 4, Icon = "✨",
                CurrentHP = 28, MaxHP = 32, ArmorClass = 16, Experience = 3800,
                Strength = 13, Dexterity = 10, Constitution = 14, Intelligence = 12, Wisdom = 16, Charisma = 14,
                Campaign = "L'Éveil du Dragon" },

            new() { Name = "Thorek Forgelame", Race = "Nain", Class = "Paladin", Level = 6, Icon = "⚡",
                CurrentHP = 72, MaxHP = 78, ArmorClass = 20, Experience = 9500,
                Strength = 17, Dexterity = 8, Constitution = 16, Intelligence = 10, Wisdom = 12, Charisma = 15,
                Campaign = "Chroniques de Waterdeep" },

            new() { Name = "Zara l'Ombre", Race = "Halfelin", Class = "Voleur", Level = 3, Icon = "🗡️",
                CurrentHP = 24, MaxHP = 26, ArmorClass = 14, Experience = 2100,
                Strength = 9, Dexterity = 17, Constitution = 12, Intelligence = 14, Wisdom = 13, Charisma = 11,
                Campaign = "La Malédiction de Strahd" }
        };
    }

    private string GetClassTheme(string characterClass)
    {
        return characterClass switch
        {
            "Guerrier" => "warrior",
            "Magicien" => "wizard",
            "Rôdeur" => "ranger",
            "Clerc" => "cleric",
            "Paladin" => "paladin",
            "Voleur" => "rogue",
            "Barbare" => "barbarian",
            "Barde" => "bard",
            _ => "default"
        };
    }

    public class CharacterData
    {
        public string Name { get; set; } = "";
        public string Race { get; set; } = "";
        public string Class { get; set; } = "";
        public int Level { get; set; }
        public string Icon { get; set; } = "";
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }
        public int ArmorClass { get; set; }
        public int Experience { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
        public string Campaign { get; set; } = "";
    }
}