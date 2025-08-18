using Microsoft.AspNetCore.Components;
using Cdm.Web.Services.Character;
using Cdm.Web.Services.Authentication;
using Cdm.Web.Models;

namespace Cdm.Web.Components.Pages;

public partial class Characters : ComponentBase
{
    [Inject] private ICharacterService CharacterService { get; set; } = default!;
    [Inject] private IAuthenticationService AuthService { get; set; } = default!;
    [Inject] private ILogger<Characters> Logger { get; set; } = default!;

    private List<Models.Character> characters = new();
    private bool isLoading = true;
    private string errorMessage = "";

    protected override async Task OnInitializedAsync()
    {
        await LoadCharacters();
    }

    private async Task LoadCharacters()
    {
        isLoading = true;
        errorMessage = "";
        StateHasChanged();

        try
        {
            // Pour l'instant, on utilise un userId fictif
            // Dans une vraie application, on récupérerait l'ID de l'utilisateur connecté
            var userId = 1; // TODO: Récupérer l'ID réel de l'utilisateur

            var result = await CharacterService.GetCharactersAsync(userId);
            
            if (result.Success && result.Data != null)
            {
                characters = result.Data;
            }
            else
            {
                // Si pas de personnages ou erreur API, utiliser les données d'exemple
                characters = GetExampleCharacters();
                if (!result.Success)
                {
                    Logger.LogWarning("Impossible de charger les personnages depuis l'API: {Message}", result.Message);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erreur lors du chargement des personnages");
            characters = GetExampleCharacters(); // Fallback vers les données d'exemple
            errorMessage = "Erreur lors du chargement des personnages. Affichage des données d'exemple.";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private List<Models.Character> GetExampleCharacters()
    {
        return new List<Models.Character>
        {
            new() { Id = 1, Name = "Thorin Forgefer", CharacterClass = "Guerrier", Race = "Nain", Level = 3, HitPoints = 33, MaxHitPoints = 40, ArmorClass = 15 },
            new() { Id = 2, Name = "Elara Luneverte", CharacterClass = "Rôdeuse", Race = "Elfe", Level = 4, HitPoints = 41, MaxHitPoints = 48, ArmorClass = 16 },
            new() { Id = 3, Name = "Gareth le Brave", CharacterClass = "Paladin", Race = "Humain", Level = 5, HitPoints = 49, MaxHitPoints = 58, ArmorClass = 17 },
            new() { Id = 4, Name = "Mystral Ombrevent", CharacterClass = "Mage", Race = "Demi-Elfe", Level = 6, HitPoints = 57, MaxHitPoints = 68, ArmorClass = 18 },
            new() { Id = 5, Name = "Lyanna Coeurvaillant", CharacterClass = "Barde", Race = "Halfeline", Level = 7, HitPoints = 65, MaxHitPoints = 78, ArmorClass = 19 },
            new() { Id = 6, Name = "Drakmor Flammenoire", CharacterClass = "Sorcier", Race = "Tieffelin", Level = 8, HitPoints = 73, MaxHitPoints = 88, ArmorClass = 20 }
        };
    }

    private string GetCharacterIcon(Models.Character character)
    {
        return character.CharacterClass?.ToLower() switch
        {
            "guerrier" => "🛡️",
            "rôdeuse" or "rodeur" => "🏹",
            "paladin" => "⚔️",
            "mage" or "magicien" => "🔮",
            "barde" => "🎵",
            "sorcier" => "🔥",
            _ => "⚔️"
        };
    }

    private async Task CreateNewCharacter()
    {
        // TODO: Implémenter la création de personnage
        Logger.LogInformation("Création d'un nouveau personnage demandée");
    }

    private async Task EditCharacter(int characterId)
    {
        // TODO: Implémenter l'édition de personnage
        Logger.LogInformation("Édition du personnage {CharacterId} demandée", characterId);
    }

    private async Task ViewCharacterDetails(int characterId)
    {
        // TODO: Implémenter l'affichage des détails
        Logger.LogInformation("Affichage des détails du personnage {CharacterId} demandé", characterId);
    }
}