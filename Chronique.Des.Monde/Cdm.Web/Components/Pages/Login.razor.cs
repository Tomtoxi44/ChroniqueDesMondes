using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Cdm.Web.Services.Authentication;

namespace Cdm.Web.Components.Pages;

public partial class Login : ComponentBase
{
    [Inject] private IAuthenticationService AuthService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private string errorMessage = "";

    protected override async Task OnInitializedAsync()
    {
        // Rediriger si déjà connecté
        if (AuthService.IsAuthenticated)
        {
            Navigation.NavigateTo("/");
            return;
        }

        // Vérifier s'il y a un message d'erreur dans l'URL
        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("error", out var error))
        {
            errorMessage = error.ToString() switch
            {
                "invalid_credentials" => "Nom d'utilisateur ou mot de passe incorrect.",
                "missing_fields" => "Nom d'utilisateur et mot de passe requis.",
                "server_error" => "Une erreur s'est produite lors de la connexion.",
                _ => "Erreur de connexion."
            };
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("initializeLoginAnimations");
        }
    }
}