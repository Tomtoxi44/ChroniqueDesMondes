using Cdm.Web.Models.Api;
using Cdm.Web.Services.Api;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Cdm.Web.Services.Authentication;

/// <summary>
/// Service d'authentification JWT moderne avec gestion des tokens
/// </summary>
public interface IJwtAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<UserDto?> GetCurrentUserAsync();
    string? GetStoredToken();
    event EventHandler<bool> AuthenticationStateChanged;
}

public class JwtAuthService : BaseApiService, IJwtAuthService
{
    private readonly IJSRuntime _jsRuntime;
    private UserDto? _currentUser;
    private string? _currentToken;
    private bool _isAuthenticated = false;

    public event EventHandler<bool>? AuthenticationStateChanged;

    public JwtAuthService(
        HttpClient httpClient, 
        ILogger<JwtAuthService> logger,
        IJSRuntime jsRuntime) 
        : base(httpClient, logger)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Authentifie un utilisateur avec email/mot de passe
    /// </summary>
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            _logger.LogInformation("🔐 Tentative de connexion pour {Email}", request.Email);

            var response = await PostAsync<LoginRequest, AuthResponse>("/api/auth/login", request);

            if (response?.Success == true && !string.IsNullOrEmpty(response.Token))
            {
                // Stocker le token et les infos utilisateur
                await StoreTokenAsync(response.Token);
                
                _currentUser = response.User;
                _currentToken = response.Token;
                _isAuthenticated = true;

                // Configurer les headers pour les futures requêtes
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", response.Token);

                _logger.LogInformation("✅ Connexion réussie pour {Email}", request.Email);
                
                // Notifier le changement d'état
                AuthenticationStateChanged?.Invoke(this, true);
                
                return response;
            }
            
            _logger.LogWarning("❌ Échec de connexion pour {Email}: {Error}", request.Email, response?.Error);
            return response ?? new AuthResponse { Success = false, Error = "Erreur de connexion" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erreur lors de la connexion pour {Email}", request.Email);
            return new AuthResponse { Success = false, Error = "Erreur technique lors de la connexion" };
        }
    }

    /// <summary>
    /// Inscrit un nouvel utilisateur
    /// </summary>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            _logger.LogInformation("📝 Tentative d'inscription pour {Email}", request.Email);

            var response = await PostAsync<RegisterRequest, AuthResponse>("/api/auth/register", request);

            if (response?.Success == true && !string.IsNullOrEmpty(response.Token))
            {
                // Stocker le token et les infos utilisateur
                await StoreTokenAsync(response.Token);
                
                _currentUser = response.User;
                _currentToken = response.Token;
                _isAuthenticated = true;

                // Configurer les headers pour les futures requêtes
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", response.Token);

                _logger.LogInformation("✅ Inscription réussie pour {Email}", request.Email);
                
                AuthenticationStateChanged?.Invoke(this, true);
                
                return response;
            }
            
            _logger.LogWarning("❌ Échec d'inscription pour {Email}: {Error}", request.Email, response?.Error);
            return response ?? new AuthResponse { Success = false, Error = "Erreur d'inscription" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erreur lors de l'inscription pour {Email}", request.Email);
            return new AuthResponse { Success = false, Error = "Erreur technique lors de l'inscription" };
        }
    }

    /// <summary>
    /// Déconnecte l'utilisateur
    /// </summary>
    public async Task LogoutAsync()
    {
        try
        {
            _logger.LogInformation("🚪 Déconnexion utilisateur");

            // Supprimer le token du stockage
            await RemoveTokenAsync();
            
            // Nettoyer l'état local
            _currentUser = null;
            _currentToken = null;
            _isAuthenticated = false;

            // Supprimer les headers d'autorisation
            _httpClient.DefaultRequestHeaders.Authorization = null;

            _logger.LogInformation("✅ Déconnexion réussie");
            
            AuthenticationStateChanged?.Invoke(this, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erreur lors de la déconnexion");
        }
    }

    /// <summary>
    /// Vérifie si l'utilisateur est authentifié
    /// </summary>
    public async Task<bool> IsAuthenticatedAsync()
    {
        // Si on a déjà un token et utilisateur en mémoire
        if (_isAuthenticated && _currentUser != null && !string.IsNullOrEmpty(_currentToken))
        {
            return true;
        }

        // Vérifier s'il y a un token stocké
        var storedToken = await GetStoredTokenAsync();
        if (!string.IsNullOrEmpty(storedToken))
        {
            // Valider le token avec l'API
            if (await ValidateTokenAsync(storedToken))
            {
                _currentToken = storedToken;
                _isAuthenticated = true;
                
                // Configurer les headers
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", storedToken);
                
                // Récupérer les infos utilisateur
                _currentUser = await FetchCurrentUserAsync();
                
                return true;
            }
            else
            {
                // Token invalide, le supprimer
                await RemoveTokenAsync();
            }
        }

        return false;
    }

    /// <summary>
    /// Récupère l'utilisateur actuellement connecté
    /// </summary>
    public async Task<UserDto?> GetCurrentUserAsync()
    {
        if (_currentUser != null)
        {
            return _currentUser;
        }

        if (await IsAuthenticatedAsync())
        {
            return _currentUser;
        }

        return null;
    }

    /// <summary>
    /// Récupère le token stocké
    /// </summary>
    public string? GetStoredToken()
    {
        return _currentToken;
    }

    #region Token Management

    /// <summary>
    /// Stocke le token JWT dans sessionStorage
    /// </summary>
    private async Task StoreTokenAsync(string token)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "cdm_auth_token", token);
            _logger.LogDebug("🔒 Token stocké avec succès");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erreur lors du stockage du token");
        }
    }

    /// <summary>
    /// Récupère le token stocké depuis sessionStorage
    /// </summary>
    private async Task<string?> GetStoredTokenAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", "cdm_auth_token");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erreur lors de la récupération du token");
            return null;
        }
    }

    /// <summary>
    /// Supprime le token stocké
    /// </summary>
    private async Task RemoveTokenAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "cdm_auth_token");
            _logger.LogDebug("🗑️ Token supprimé avec succès");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erreur lors de la suppression du token");
        }
    }

    /// <summary>
    /// Valide un token avec l'API backend
    /// </summary>
    private async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            // Temporairement configurer les headers pour cette requête
            var originalAuth = _httpClient.DefaultRequestHeaders.Authorization;
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("/api/auth/validate");
            
            // Restaurer les headers originaux
            _httpClient.DefaultRequestHeaders.Authorization = originalAuth;

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erreur lors de la validation du token");
            return false;
        }
    }

    /// <summary>
    /// Récupère les informations de l'utilisateur actuel depuis l'API
    /// </summary>
    private async Task<UserDto?> FetchCurrentUserAsync()
    {
        try
        {
            return await GetAsync<UserDto>("/api/auth/me");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erreur lors de la récupération des informations utilisateur");
            return null;
        }
    }

    #endregion
}