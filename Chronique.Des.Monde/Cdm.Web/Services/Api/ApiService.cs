using Cdm.Web.Models;
using System.Text.Json;
using System.Text;

namespace Cdm.Web.Services.Api;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _fallbackBaseUrl = "https://localhost:7428";

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        // Vérifier et corriger la BaseAddress si elle est null
        if (_httpClient.BaseAddress == null)
        {
            _logger.LogWarning("⚠️ HttpClient.BaseAddress est null, utilisation de l'URL de fallback: {FallbackUrl}", _fallbackBaseUrl);
            _httpClient.BaseAddress = new Uri(_fallbackBaseUrl);
        }

        // Log the base address for debugging
        _logger.LogInformation("✅ ApiService initialized with BaseAddress: {BaseAddress}", _httpClient.BaseAddress);
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            EnsureBaseAddress();
            
            _logger.LogInformation("🔐 Tentative de connexion pour {Email} vers {BaseAddress}/login", request.Email, _httpClient.BaseAddress);
            
            var json = JsonSerializer.Serialize(new { email = request.Email, password = request.Password }, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("📡 Réponse de l'API login: Status={StatusCode}, Content Length={ContentLength}", 
                response.StatusCode, responseContent.Length);

            if (response.IsSuccessStatusCode)
            {
                var loginData = JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonOptions);
                return new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Data = loginData,
                    Message = "Connexion réussie"
                };
            }
            else
            {
                _logger.LogWarning("❌ Échec de la connexion: Status={StatusCode}, Content={Content}", 
                    response.StatusCode, responseContent);
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = response.StatusCode == System.Net.HttpStatusCode.Unauthorized 
                        ? "Email ou mot de passe incorrect" 
                        : "Erreur lors de la connexion"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erreur lors de la connexion pour {Email} vers {BaseAddress}", request.Email, _httpClient.BaseAddress);
            return new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "Erreur de connexion au serveur"
            };
        }
    }

    public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            EnsureBaseAddress();
            
            _logger.LogInformation("📝 Tentative d'inscription pour {Email} vers {BaseAddress}/register", request.UserEmail, _httpClient.BaseAddress);
            
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/register", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("📡 Réponse de l'API register: Status={StatusCode}, Content Length={ContentLength}", 
                response.StatusCode, responseContent.Length);

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<RegisterResponse>
                {
                    Success = true,
                    Data = new RegisterResponse { Success = true, Message = "Inscription réussie" },
                    Message = "Compte créé avec succès"
                };
            }
            else
            {
                _logger.LogWarning("❌ Échec de l'inscription: Status={StatusCode}, Content={Content}", 
                    response.StatusCode, responseContent);
                return new ApiResponse<RegisterResponse>
                {
                    Success = false,
                    Message = "Erreur lors de l'inscription"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erreur lors de l'inscription pour {Email} vers {BaseAddress}", request.UserEmail, _httpClient.BaseAddress);
            return new ApiResponse<RegisterResponse>
            {
                Success = false,
                Message = "Erreur de connexion au serveur"
            };
        }
    }

    private void EnsureBaseAddress()
    {
        if (_httpClient.BaseAddress == null)
        {
            _logger.LogWarning("🔧 BaseAddress est null, correction automatique avec: {FallbackUrl}", _fallbackBaseUrl);
            _httpClient.BaseAddress = new Uri(_fallbackBaseUrl);
        }
    }
}