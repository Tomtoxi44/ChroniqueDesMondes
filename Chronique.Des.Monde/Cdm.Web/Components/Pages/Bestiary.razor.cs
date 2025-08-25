using Microsoft.AspNetCore.Components;

namespace Cdm.Web.Components.Pages;

public partial class Bestiary : ComponentBase
{
    private string searchTerm = "";
    private string selectedType = "";
    private string selectedChallenge = "";
    private List<Creature> allCreatures = new();
    private List<Creature> filteredCreatures = new();

    protected override void OnInitialized()
    {
        LoadCreatures();
        filteredCreatures = allCreatures;
    }

    protected override void OnParametersSet()
    {
        FilterCreatures();
    }

    private void LoadCreatures()
    {
        allCreatures = new List<Creature>
        {
            // Dragons
            new() { Name = "Dragon Rouge Ancien", Type = "Dragon", Size = "Gigantesque", Icon = "🐲", Challenge = 17, 
                ArmorClass = 22, HitPoints = "546 (28d20 + 252)", Speed = "12m, vol 24m",
                Strength = 30, Dexterity = 10, Constitution = 29, Intelligence = 18, Wisdom = 15, Charisma = 23,
                Description = "Un dragon rouge ancien d'une puissance terrifiant, maître absolu du feu et de la destruction.",
                SpecialAbilities = new[] { "Immunité au feu", "Vision aveugle 18m", "Présence terrifiante", "Résistance légendaire (3/jour)" },
                Actions = new[] { "Morsure: +17, 2d10+10 perforant + 2d6 feu", "Griffes: +17, 2d6+10 tranchant", "Souffle de feu (Recharge 5-6): 26d6 feu" } },

            new() { Name = "Dragonnet d'Or", Type = "Dragon", Size = "Moyen", Icon = "🐉", Challenge = 3,
                ArmorClass = 17, HitPoints = "60 (8d8 + 24)", Speed = "9m, vol 18m, nage 9m",
                Strength = 19, Dexterity = 14, Constitution = 17, Intelligence = 14, Wisdom = 11, Charisma = 16,
                Description = "Un jeune dragon doré noble et juste, allié naturel des forces du bien.",
                SpecialAbilities = new[] { "Amphibie", "Vision aveugle 3m", "Vision dans le noir 18m" },
                Actions = new[] { "Morsure: +6, 1d10+4 perforant", "Souffle affaiblissant (Recharge 5-6): 4d6 force" } },

            // Morts-vivants
            new() { Name = "Liche", Type = "Mort-vivant", Size = "Moyen", Icon = "💀", Challenge = 21,
                ArmorClass = 17, HitPoints = "165 (22d8 + 66)", Speed = "9m, vol 9m",
                Strength = 11, Dexterity = 16, Constitution = 16, Intelligence = 20, Wisdom = 14, Charisma = 16,
                Description = "Un lanceur de sorts mort-vivant d'une puissance immense, ayant transcendé la mort pour l'immortalité.",
                SpecialAbilities = new[] { "Résistance légendaire (3/jour)", "Régénération", "Actions légendaires", "Immunité charme/épuisement/peur" },
                Actions = new[] { "Contact paralysant: +12, 3d6 froid + paralysie", "Sorts niveau 9", "Désintégration", "Nuée de météores" } },

            new() { Name = "Zombie", Type = "Mort-vivant", Size = "Moyen", Icon = "🧟", Challenge = 1,
                ArmorClass = 8, HitPoints = "22 (3d8 + 9)", Speed = "6m",
                Strength = 13, Dexterity = 6, Constitution = 16, Intelligence = 3, Wisdom = 6, Charisma = 5,
                Description = "Un cadavre animé par une nécromancie sombre, privé d'intelligence mais implacable.",
                SpecialAbilities = new[] { "Résistance endurance (1 PV au lieu de mourir)" },
                Actions = new[] { "Coup: +3, 1d6+1 contondant" } },

            // Démons
            new() { Name = "Balor", Type = "Démon", Size = "Très Grand", Icon = "👹", Challenge = 19,
                ArmorClass = 19, HitPoints = "262 (21d12 + 126)", Speed = "12m, vol 24m",
                Strength = 26, Dexterity = 15, Constitution = 22, Intelligence = 20, Wisdom = 16, Charisma = 22,
                Description = "Un démon majeur des Abysses, général des armées démoniaques et incarnation de la destruction.",
                SpecialAbilities = new[] { "Aura de feu", "Résistance à la magie", "Explosion à la mort" },
                Actions = new[] { "Épée longue: +14, 2d8+8 tranchant + 3d8 feu", "Fouet: +14, 2d6+8 tranchant + 3d6 feu + attraction" } },

            new() { Name = "Diablotin", Type = "Démon", Size = "Très Petit", Icon = "👺", Challenge = 1,
                ArmorClass = 13, HitPoints = "10 (3d4 + 3)", Speed = "6m, vol 12m",
                Strength = 6, Dexterity = 17, Constitution = 13, Intelligence = 11, Wisdom = 12, Charisma = 14,
                Description = "Un petit démon espiègle servant souvent de familier aux sorciers maléfiques.",
                SpecialAbilities = new[] { "Résistance à la magie", "Invisibilité à volonté" },
                Actions = new[] { "Dard: +5, 1d4+3 perforant + poison" } },

            // Bêtes
            new() { Name = "Ours-Hibou", Type = "Bête", Size = "Grand", Icon = "🦉", Challenge = 3,
                ArmorClass = 13, HitPoints = "59 (7d10 + 21)", Speed = "12m, vol 18m",
                Strength = 20, Dexterity = 15, Constitution = 17, Intelligence = 3, Wisdom = 13, Charisma = 7,
                Description = "Une créature majestueuse combinant la force de l'ours et la grâce du hibou.",
                SpecialAbilities = new[] { "Vision dans le noir 18m", "Odorat et ouïe aiguisés" },
                Actions = new[] { "Griffes: +7, 1d10+5 tranchant", "Bec: +7, 1d8+5 perforant" } },

            new() { Name = "Loup Sinistre", Type = "Bête", Size = "Grand", Icon = "🐺", Challenge = 1,
                ArmorClass = 14, HitPoints = "37 (5d10 + 10)", Speed = "15m",
                Strength = 17, Dexterity = 15, Constitution = 15, Intelligence = 3, Wisdom = 12, Charisma = 7,
                Description = "Un loup gigantesque aux yeux rougeoyants, plus intelligent et féroce qu'un loup ordinaire.",
                SpecialAbilities = new[] { "Odorat et ouïe aiguisés", "Tactique de meute" },
                Actions = new[] { "Morsure: +5, 2d6+3 perforant + renversement" } },

            // Élémentaires
            new() { Name = "Élémentaire de Feu", Type = "Élémentaire", Size = "Grand", Icon = "🔥", Challenge = 5,
                ArmorClass = 13, HitPoints = "102 (12d10 + 36)", Speed = "15m",
                Strength = 10, Dexterity = 17, Constitution = 16, Intelligence = 6, Wisdom = 10, Charisma = 7,
                Description = "Une incarnation pure du feu élémentaire, corps de flammes dansantes et d'énergie destructrice.",
                SpecialAbilities = new[] { "Forme de feu", "Illumination", "Vulnérabilité au froid", "Immunité au feu" },
                Actions = new[] { "Contact: +6, 2d6+3 feu + inflammation" } },

            new() { Name = "Élémentaire d'Air", Type = "Élémentaire", Size = "Grand", Icon = "💨", Challenge = 5,
                ArmorClass = 15, HitPoints = "90 (12d10 + 24)", Speed = "0m, vol 27m",
                Strength = 14, Dexterity = 20, Constitution = 14, Intelligence = 6, Wisdom = 10, Charisma = 6,
                Description = "Un tourbillon conscient de vents violents et de nuages, maître des cieux.",
                SpecialAbilities = new[] { "Forme d'air", "Tourbillon" },
                Actions = new[] { "Coup: +8, 2d8+5 contondant" } },

            // Géants
            new() { Name = "Géant des Collines", Type = "Géant", Size = "Très Grand", Icon = "👨‍🦲", Challenge = 5,
                ArmorClass = 13, HitPoints = "105 (10d12 + 40)", Speed = "12m",
                Strength = 21, Dexterity = 8, Constitution = 19, Intelligence = 5, Wisdom = 9, Charisma = 6,
                Description = "Un géant primitif et brutal, vivant dans les collines et se nourrissant de tout ce qu'il trouve.",
                SpecialAbilities = new[] { "Odorat aiguisé" },
                Actions = new[] { "Massue: +8, 3d8+5 contondant", "Rocher: +8, 3d10+5 contondant" } },

            new() { Name = "Géant du Feu", Type = "Géant", Size = "Très Grand", Icon = "🔥", Challenge = 9,
                ArmorClass = 18, HitPoints = "162 (13d12 + 78)", Speed = "9m",
                Strength = 25, Dexterity = 9, Constitution = 23, Intelligence = 10, Wisdom = 14, Charisma = 13,
                Description = "Un géant maître de la forge et du feu, créateur d'armes légendaires.",
                SpecialAbilities = new[] { "Immunité au feu" },
                Actions = new[] { "Épée à deux mains: +11, 6d6+7 tranchant", "Rocher: +11, 4d10+7 contondant + feu" } },

            // Fées
            new() { Name = "Dryade", Type = "Fée", Size = "Moyen", Icon = "🧚‍♀️", Challenge = 1,
                ArmorClass = 11, HitPoints = "22 (5d8)", Speed = "9m",
                Strength = 10, Dexterity = 12, Constitution = 11, Intelligence = 14, Wisdom = 15, Charisma = 18,
                Description = "Un esprit de la nature lié à un arbre spécifique, gardienne de la forêt.",
                SpecialAbilities = new[] { "Sorts innés", "Fusion avec l'arbre" },
                Actions = new[] { "Bâton: +2, 1d4 contondant", "Charme fée" } },

            new() { Name = "Pixie", Type = "Fée", Size = "Très Petit", Icon = "🧚", Challenge = 1,
                ArmorClass = 15, HitPoints = "1 (1d4 - 1)", Speed = "3m, vol 9m",
                Strength = 2, Dexterity = 20, Constitution = 8, Intelligence = 10, Wisdom = 14, Charisma = 15,
                Description = "Une minuscule fée espiègle aux pouvoirs magiques surprenants malgré sa taille.",
                SpecialAbilities = new[] { "Sorts innés", "Résistance à la magie" },
                Actions = new[] { "Épée supérieure: +4, 1 perforant + poison", "Arc supérieur: +4, 1 perforant + sommeil" } }
        };
    }

    private void FilterCreatures()
    {
        filteredCreatures = allCreatures.Where(creature =>
            (string.IsNullOrEmpty(searchTerm) || creature.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(selectedType) || creature.Type == selectedType) &&
            (string.IsNullOrEmpty(selectedChallenge) || IsInChallengeRange(creature.Challenge, selectedChallenge))
        ).ToList();
        
        StateHasChanged();
    }

    private bool IsInChallengeRange(int challenge, string range)
    {
        return range switch
        {
            "1-5" => challenge >= 1 && challenge <= 5,
            "6-10" => challenge >= 6 && challenge <= 10,
            "11-15" => challenge >= 11 && challenge <= 15,
            "16-20" => challenge >= 16 && challenge <= 20,
            "21+" => challenge >= 21,
            _ => true
        };
    }

    private string GetChallengeClass(int challenge)
    {
        return challenge switch
        {
            <= 2 => "easy",
            <= 5 => "medium",
            <= 10 => "hard",
            <= 15 => "deadly",
            <= 20 => "epic",
            _ => "legendary"
        };
    }

    public class Creature
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string Size { get; set; } = "";
        public string Icon { get; set; } = "";
        public int Challenge { get; set; }
        public int ArmorClass { get; set; }
        public string HitPoints { get; set; } = "";
        public string Speed { get; set; } = "";
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
        public string Description { get; set; } = "";
        public string[] SpecialAbilities { get; set; } = Array.Empty<string>();
        public string[] Actions { get; set; } = Array.Empty<string>();
    }
}