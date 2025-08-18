using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Cdm.Web.Services.Api;
using Cdm.Web.Models;

namespace Cdm.Web.Controllers;

[Route("auth")]
public class AuthController : Controller
{
    private readonly IApiService _apiService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IApiService apiService, ILogger<AuthController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password, [FromForm] string? returnUrl = null)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Redirect("/login?error=missing_fields");
            }

            var loginRequest = new LoginRequest
            {
                Email = username,
                Password = password
            };

            var result = await _apiService.LoginAsync(loginRequest);
            
            if (!result.Success || result.Data?.Token == null)
            {
                _logger.LogWarning("Échec de connexion pour {Username}: {Message}", username, result.Message);
                return Redirect("/login?error=invalid_credentials");
            }

            // Créer les claims avec les informations utilisateur
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, username),
                new(ClaimTypes.Email, username),
                new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new("DisplayName", username),
                new("Token", result.Data.Token)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("Connexion réussie pour {Username}", username);
            
            // Redirection vers la page demandée ou vers l'accueil
            return Redirect(returnUrl ?? "/");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la connexion pour {Username}", username);
            return Redirect("/login?error=server_error");
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Déconnexion réussie");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la déconnexion");
        }
        
        return Redirect("/");
    }
}