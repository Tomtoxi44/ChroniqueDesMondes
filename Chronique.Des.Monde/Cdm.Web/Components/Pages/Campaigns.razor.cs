using Microsoft.AspNetCore.Components;

namespace Cdm.Web.Components.Pages;

public partial class Campaigns : ComponentBase
{
    private List<Campaign> campaigns = new();

    protected override void OnInitialized()
    {
        LoadCampaigns();
    }

    private void LoadCampaigns()
    {
        campaigns = new List<Campaign>
        {
            new() { Name = "Les Terres Oubliées", 
                Description = "Une campagne épique dans un monde post-apocalyptique où la magie a refaçonné la réalité.",
                Icon = "🏰", Status = "En cours", GameMaster = "Maître Aldric", 
                PlayerCount = 4, MaxPlayers = 6, System = "D&D 5e",
                LastSession = "Il y a 3 jours", Progress = 65, CurrentSession = 12, CurrentArc = "II" },

            new() { Name = "L'Éveil du Dragon", 
                Description = "Les héros doivent empêcher le réveil d'un dragon ancien qui menace de détruire le royaume.",
                Icon = "🐲", Status = "En attente", GameMaster = "Dame Lyralei", 
                PlayerCount = 3, MaxPlayers = 5, System = "D&D 5e",
                LastSession = "Demain 20h", Progress = 25, CurrentSession = 5, CurrentArc = "I" },

            new() { Name = "Chroniques de Waterdeep", 
                Description = "Intrigue politique et aventures urbaines dans la plus grande cité de la Côte des Épées.",
                Icon = "🏛️", Status = "Terminée", GameMaster = "Sage Gandouin", 
                PlayerCount = 5, MaxPlayers = 5, System = "D&D 5e",
                LastSession = "Il y a 2 semaines", Progress = 100, CurrentSession = 24, CurrentArc = "Final" },

            new() { Name = "La Malédiction de Strahd", 
                Description = "Horror gothique dans le domaine sombre de Barovia, sous l'emprise du vampire Strahd.",
                Icon = "🦇", Status = "En cours", GameMaster = "Comte Ravenloft", 
                PlayerCount = 4, MaxPlayers = 4, System = "D&D 5e",
                LastSession = "Hier", Progress = 80, CurrentSession = 18, CurrentArc = "III" },

            new() { Name = "Spelljammer - Pirates de l'Espace", 
                Description = "Aventures cosmiques à bord de vaisseaux magiques naviguant entre les sphères cristallines.",
                Icon = "🚀", Status = "Planifiée", GameMaster = "Capitaine Voidrunner", 
                PlayerCount = 2, MaxPlayers = 6, System = "D&D 5e",
                LastSession = "Prochainement", Progress = 0, CurrentSession = 0, CurrentArc = "Prologue" }
        };
    }

    private string GetStatusClass(string status)
    {
        return status switch
        {
            "En cours" => "active",
            "En attente" => "pending",
            "Planifiée" => "planned",
            "Terminée" => "completed",
            _ => "unknown"
        };
    }

    public class Campaign
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Status { get; set; } = "";
        public string GameMaster { get; set; } = "";
        public int PlayerCount { get; set; }
        public int MaxPlayers { get; set; }
        public string System { get; set; } = "";
        public string LastSession { get; set; } = "";
        public int Progress { get; set; }
        public int CurrentSession { get; set; }
        public string CurrentArc { get; set; } = "";
    }
}