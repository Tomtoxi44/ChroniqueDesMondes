using Microsoft.AspNetCore.Components;

namespace Cdm.Web.Components.Pages;

public partial class Equipment : ComponentBase
{
    private string searchTerm = "";
    private string selectedType = "";
    private string selectedRarity = "";
    private List<EquipmentItem> allEquipment = new();
    private List<EquipmentItem> filteredEquipment = new();

    protected override void OnInitialized()
    {
        LoadEquipment();
        filteredEquipment = allEquipment;
    }

    protected override void OnParametersSet()
    {
        FilterEquipment();
    }

    private void LoadEquipment()
    {
        allEquipment = new List<EquipmentItem>
        {
            // Armes
            new() { Name = "Épée Longue +1", Type = "Arme", Subtype = "Épée", Icon = "⚔️", Rarity = "Peu Commun", 
                Damage = "1d8+1 tranchant", Weight = 1.5m, Value = 500, 
                Description = "Une épée longue finement forgée avec une lame enchantée qui brille d'une lueur argentée." },
            
            new() { Name = "Lame de Flammes", Type = "Arme", Subtype = "Épée", Icon = "🔥", Rarity = "Rare", 
                Damage = "1d8 tranchant + 1d6 feu", Weight = 1.5m, Value = 2000, 
                Description = "Une épée dont la lame s'embrase quand elle est dégainée.", 
                MagicalProperties = new[] { "Inflige 1d6 dégâts de feu supplémentaires", "Émet une lumière vive dans un rayon de 6m" } },
            
            new() { Name = "Arc Elfique", Type = "Arme", Subtype = "Arc", Icon = "🏹", Rarity = "Peu Commun", 
                Damage = "1d8+1 perforant", Weight = 1m, Value = 800, 
                Description = "Un arc élégamment courbé, taillé dans le bois d'un arbre millénaire par les maîtres artisans elfes.", 
                MagicalProperties = new[] { "Portée augmentée de 50%", "Ignore la résistance aux dégâts perforants" } },

            new() { Name = "Marteau de Guerre du Tonnerre", Type = "Arme", Subtype = "Marteau", Icon = "🔨", Rarity = "Très Rare", 
                Damage = "1d8+2 contondant + tonnerre", Weight = 2m, Value = 5000, 
                Description = "Un marteau de guerre nain forgé avec l'essence même de l'orage.", 
                MagicalProperties = new[] { "Coup critique sur 19-20", "Peut lancer Appel de la Foudre 1/jour" }, 
                Requirements = "Force 15 ou plus" },

            // Armures
            new() { Name = "Armure de Cuir Clouté +1", Type = "Armure", Subtype = "Cuir", Icon = "🦺", Rarity = "Peu Commun", 
                ArmorClass = "12 + Mod.Dex + 1", Weight = 6m, Value = 700, 
                Description = "Une armure de cuir renforcée avec des clous métalliques enchantés." },
            
            new() { Name = "Cotte de Mailles Elfique", Type = "Armure", Subtype = "Mailles", Icon = "⛓️", Rarity = "Rare", 
                ArmorClass = "14 + Mod.Dex (max 2)", Weight = 8m, Value = 3000, 
                Description = "Une cotte de mailles tissée avec des fils d'argent enchantés, légère comme une plume.", 
                MagicalProperties = new[] { "Pas de désavantage en Discrétion", "Résistance aux attaques perforantes" } },
            
            new() { Name = "Harnois du Dragon", Type = "Armure", Subtype = "Lourde", Icon = "🛡️", Rarity = "Légendaire", 
                ArmorClass = "18", Weight = 30m, Value = 20000, 
                Description = "Une armure complète forgée avec les écailles d'un dragon rouge ancien.", 
                MagicalProperties = new[] { "Immunité aux dégâts de feu", "Peut voler 1 heure/jour", "Résistance à la magie" }, 
                Requirements = "Force 17, Maîtrise des armures lourdes" },

            // Boucliers
            new() { Name = "Bouclier +2", Type = "Bouclier", Icon = "🛡️", Rarity = "Rare", 
                ArmorClass = "+2 CA", Weight = 3m, Value = 1500, 
                Description = "Un bouclier en acier renforcé gravé de runes de protection." },
            
            new() { Name = "Égide du Gardien", Type = "Bouclier", Icon = "✨", Rarity = "Très Rare", 
                ArmorClass = "+3 CA", Weight = 3m, Value = 8000, 
                Description = "Un bouclier légendaire ayant appartenu à un paladin célèbre.", 
                MagicalProperties = new[] { "Réflexion des sorts (1/jour)", "Guérison 2d4+2 PV (3/jour)", "Aura de protection 3m" }, 
                Requirements = "Alignement Bon" },

            // Accessoires
            new() { Name = "Anneau de Protection", Type = "Accessoire", Subtype = "Anneau", Icon = "💍", Rarity = "Rare", 
                Weight = 0, Value = 3500, 
                Description = "Un anneau serti d'un saphir qui pulse d'une douce lueur protectrice.", 
                MagicalProperties = new[] { "+1 CA et jets de sauvegarde" } },
            
            new() { Name = "Amulette de Santé", Type = "Accessoire", Subtype = "Amulette", Icon = "🔮", Rarity = "Rare", 
                Weight = 0.1m, Value = 4000, 
                Description = "Une amulette en forme de cœur qui bat doucement contre la poitrine.", 
                MagicalProperties = new[] { "Constitution fixée à 19", "Immunité aux maladies" } },
            
            new() { Name = "Cape Elfique", Type = "Accessoire", Subtype = "Cape", Icon = "🧥", Rarity = "Peu Commun", 
                Weight = 0.5m, Value = 1200, 
                Description = "Une cape tissée avec de la soie d'araignée phase, presque transparente.", 
                MagicalProperties = new[] { "Avantage aux jets de Discrétion", "Résistance aux sorts de divination" } },

            // Objets Magiques
            new() { Name = "Baguette de Projectiles Magiques", Type = "Objet Magique", Subtype = "Baguette", Icon = "🪄", Rarity = "Peu Commun", 
                Weight = 0.5m, Value = 2000, 
                Description = "Une baguette d'ébène incrustée d'éclats de cristal qui scintillent d'énergie magique.", 
                MagicalProperties = new[] { "7 charges", "Lance Projectile Magique (1-3 charges)", "Récupère 1d6+1 charges à l'aube" } },
            
            new() { Name = "Sac Sans Fond", Type = "Objet Magique", Subtype = "Merveilleux", Icon = "🎒", Rarity = "Peu Commun", 
                Weight = 7.5m, Value = 2500, 
                Description = "Un sac de toile qui semble ordinaire mais cache un espace extra-dimensionnel.", 
                MagicalProperties = new[] { "Contient 500 kg dans 60 litres", "Récupération instantanée des objets" } },
            
            new() { Name = "Orbe de Domination Draconique", Type = "Objet Magique", Subtype = "Orbe", Icon = "🔮", Rarity = "Légendaire", 
                Weight = 1.5m, Value = 50000, 
                Description = "Un orbe de cristal noir contenant l'essence d'un dragon-roi ancien.", 
                MagicalProperties = new[] { "Contrôle des dragons (1/jour)", "Souffle draconique (3/jour)", "Vol permanent", "Télépathie draconique" }, 
                Requirements = "Lanceur de sorts niveau 17+" }
        };
    }

    private void FilterEquipment()
    {
        filteredEquipment = allEquipment.Where(item =>
            (string.IsNullOrEmpty(searchTerm) || item.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(selectedType) || item.Type == selectedType) &&
            (string.IsNullOrEmpty(selectedRarity) || item.Rarity == selectedRarity)
        ).ToList();
        
        StateHasChanged();
    }

    private string GetRarityClass(string rarity)
    {
        return rarity switch
        {
            "Commun" => "common",
            "Peu Commun" => "uncommon",
            "Rare" => "rare",
            "Très Rare" => "very-rare",
            "Légendaire" => "legendary",
            _ => "common"
        };
    }

    public class EquipmentItem
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string Subtype { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Rarity { get; set; } = "";
        public string Damage { get; set; } = "";
        public string ArmorClass { get; set; } = "";
        public decimal Weight { get; set; }
        public int Value { get; set; }
        public string Description { get; set; } = "";
        public string[] MagicalProperties { get; set; } = Array.Empty<string>();
        public string Requirements { get; set; } = "";
    }
}