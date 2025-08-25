using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Cdm.Web.Services.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Cdm.Web.Components.Pages;

public partial class Register : ComponentBase
{
    [Inject] private IAuthenticationService AuthService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private RegisterModel registerModel = new();
    private string errorMessage = "";
    private string successMessage = "";
    private bool isLoading = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("initializeRegisterAnimations");
        }
    }

    private async Task HandleRegister()
    {
        isLoading = true;
        errorMessage = "";
        successMessage = "";
        StateHasChanged();

        try
        {
            if (registerModel.Password != registerModel.ConfirmPassword)
            {
                errorMessage = "Les mots de passe ne correspondent pas.";
                return;
            }

            var success = await AuthService.RegisterAsync(registerModel.Username!, registerModel.Email!, registerModel.Password!);
            
            if (success)
            {
                successMessage = "Compte créé avec succès ! Redirection vers la connexion...";
                await Task.Delay(2000);
                Navigation.NavigateTo("/login");
            }
            else
            {
                errorMessage = "Une erreur s'est produite lors de la création du compte.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = "Une erreur s'est produite lors de la création du compte.";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Le nom d'utilisateur doit contenir entre 3 et 50 caractères")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "L'adresse email est requise")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "La confirmation du mot de passe est requise")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vous devez accepter les conditions d'utilisation")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Vous devez accepter les conditions d'utilisation")]
        public bool AcceptTerms { get; set; }
    }
}