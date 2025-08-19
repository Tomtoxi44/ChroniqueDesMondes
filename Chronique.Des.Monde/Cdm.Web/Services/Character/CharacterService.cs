using Cdm.Web.Models;
using System.Text.Json;
using System.Text;
using Cdm.Web.Services.Authentication;

namespace Cdm.Web.Services.Character;

public class CharacterService : ICharacterService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CharacterService> _logger;
    private readonly IAuthenticationService _authService;
    private readonly JsonSerializerOptions _jsonOptions;

    public CharacterService(
        HttpClient httpClient, 
        ILogger<CharacterService> logger,
        IAuthenticationService authService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _authService = authService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<ApiResponse<List<Models.Character>>> GetCharactersAsync(int userId)
    {
        try
        {
            AddAuthorizationHeader();
            _httpClient.DefaultRequestHeaders.Add("X-GameType", "Dnd");

            var response = await _httpClient.GetAsync($"/character?userId={userId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var characters = JsonSerializer.Deserialize<List<Models.Character>>(responseContent, _jsonOptions);
                return new ApiResponse<List<Models.Character>>
                {
                    Success = true,
                    Data = characters ?? new List<Models.Character>(),
                    Message = "Personnages récupérés avec succès"
                };
            }
            else
            {
                return new ApiResponse<List<Models.Character>>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération des personnages"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des personnages pour l'utilisateur {UserId}", userId);
            return new ApiResponse<List<Models.Character>>
            {
                Success = false,
                Message = "Erreur de connexion au serveur"
            };
        }
    }

    public async Task<ApiResponse<Models.Character>> GetCharacterAsync(int characterId)
    {
        try
        {
            AddAuthorizationHeader();
            _httpClient.DefaultRequestHeaders.Add("X-GameType", "Dnd");

            var response = await _httpClient.GetAsync($"/character/{characterId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var character = JsonSerializer.Deserialize<Models.Character>(responseContent, _jsonOptions);
                return new ApiResponse<Models.Character>
                {
                    Success = true,
                    Data = character,
                    Message = "Personnage récupéré avec succès"
                };
            }
            else
            {
                return new ApiResponse<Models.Character>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération du personnage"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du personnage {CharacterId}", characterId);
            return new ApiResponse<Models.Character>
            {
                Success = false,
                Message = "Erreur de connexion au serveur"
            };
        }
    }

    public async Task<ApiResponse<Models.Character>> CreateCharacterAsync(CharacterRequest request, int userId)
    {
        try
        {
            AddAuthorizationHeader();
            _httpClient.DefaultRequestHeaders.Add("X-GameType", "Dnd");

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/character?userId={userId}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var character = JsonSerializer.Deserialize<Models.Character>(responseContent, _jsonOptions);
                return new ApiResponse<Models.Character>
                {
                    Success = true,
                    Data = character,
                    Message = "Personnage créé avec succès"
                };
            }
            else
            {
                return new ApiResponse<Models.Character>
                {
                    Success = false,
                    Message = "Erreur lors de la création du personnage"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du personnage pour l'utilisateur {UserId}", userId);
            return new ApiResponse<Models.Character>
            {
                Success = false,
                Message = "Erreur de connexion au serveur"
            };
        }
    }

    public async Task<ApiResponse<Models.Character>> CreateDndCharacterAsync(CharacterDndRequest request, int userId)
    {
        try
        {
            AddAuthorizationHeader();
            _httpClient.DefaultRequestHeaders.Add("X-GameType", "Dnd");

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/character/dnd?userId={userId}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var character = JsonSerializer.Deserialize<Models.Character>(responseContent, _jsonOptions);
                return new ApiResponse<Models.Character>
                {
                    Success = true,
                    Data = character,
                    Message = "Personnage D&D créé avec succès"
                };
            }
            else
            {
                return new ApiResponse<Models.Character>
                {
                    Success = false,
                    Message = "Erreur lors de la création du personnage D&D"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du personnage D&D pour l'utilisateur {UserId}", userId);
            return new ApiResponse<Models.Character>
            {
                Success = false,
                Message = "Erreur de connexion au serveur"
            };
        }
    }

    private void AddAuthorizationHeader()
    {
        var token = _authService.GetCurrentUserToken();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}