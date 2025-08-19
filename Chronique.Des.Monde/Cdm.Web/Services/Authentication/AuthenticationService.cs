using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Cdm.Web.Models;
using Cdm.Web.Services.Api;

namespace Cdm.Web.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IApiService _apiService;

    public AuthenticationService(
        IHttpContextAccessor httpContextAccessor, 
        ILogger<AuthenticationService> logger,
        IApiService apiService)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _apiService = apiService;
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            var loginRequest = new LoginRequest
            {
                Email = username,
                Password = password
            };

            var result = await _apiService.LoginAsync(loginRequest);
            
            if (!result.Success || result.Data?.Token == null)
            {
                _logger.LogWarning("Échec de connexion pour {Username}: {Message}", username, result.Message);
                return false;
            }

            // Vérifier si le HttpContext est disponible et si la réponse n'a pas déjà commencé
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                _logger.LogError("HttpContext non disponible lors de la connexion");
                return false;
            }

            // Vérifier si la réponse a déjà commencé
            if (httpContext.Response.HasStarted)
            {
                _logger.LogWarning("La réponse a déjà commencé, impossible de définir les cookies d'authentification");
                return false;
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
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                RedirectUri = "/"
            };

            try
            {
                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("Connexion réussie pour {Username}", username);
                return true;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Headers are read-only"))
            {
                _logger.LogWarning("Impossible de définir les cookies d'authentification: {Error}", ex.Message);
                // Dans ce cas, on stocke temporairement les informations d'authentification
                // et on demandera une redirection
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la connexion pour {Username}", username);
            return false;
        }
    }

    public async Task<bool> RegisterAsync(string username, string email, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return false;

            var registerRequest = new RegisterRequest
            {
                UserName = username,
                UserEmail = email,
                Password = password
            };

            var result = await _apiService.RegisterAsync(registerRequest);
            
            if (result.Success)
            {
                _logger.LogInformation("Inscription réussie pour {Username} ({Email})", username, email);
                return true;
            }
            else
            {
                _logger.LogWarning("Échec d'inscription pour {Username}: {Message}", username, result.Message);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'inscription pour {Username}", username);
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && !httpContext.Response.HasStarted)
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                _logger.LogInformation("Déconnexion réussie");
            }
            else
            {
                _logger.LogWarning("Impossible de déconnecter: HttpContext indisponible ou réponse déjà commencée");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la déconnexion");
        }
    }

    public async Task<ClaimsPrincipal?> GetCurrentUserAsync()
    {
        return await Task.FromResult(_httpContextAccessor.HttpContext?.User);
    }

    public string? GetCurrentUserToken()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst("Token")?.Value;
    }
}