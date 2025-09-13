using Cdm.Web.Models.Api;
using Cdm.Web.Services.Api;

namespace Cdm.Web.Services.Characters;

/// <summary>
/// Service pour la gestion des personnages D&D via l'API
/// </summary>
public interface ICharacterApiService
{
    Task<List<CharacterDto>> GetCharactersAsync();
    Task<CharacterDto?> GetCharacterAsync(int id);
    Task<CharacterDto?> CreateCharacterAsync(CreateCharacterRequest request);
    Task<CharacterDto?> UpdateCharacterAsync(int id, UpdateCharacterRequest request);
    Task<bool> DeleteCharacterAsync(int id);
}

public class CharacterApiService : BaseApiService, ICharacterApiService
{
    public CharacterApiService(HttpClient httpClient, ILogger<CharacterApiService> logger) 
        : base(httpClient, logger)
    {
    }

    /// <summary>
    /// Récupère tous les personnages de l'utilisateur connecté
    /// </summary>
    public async Task<List<CharacterDto>> GetCharactersAsync()
    {
        try
        {
            _logger.LogInformation("📋 Récupération de la liste des personnages");

            // TODO: Adapter l'endpoint selon votre API
            var characters = await GetAsync<List<CharacterDto>>("/api/characters");
            
            if (characters != null)
            {
                _logger.LogInformation("✅ {Count} personnages récupérés", characters.Count);
                return characters;
            }
            
            _logger.LogWarning("⚠️ Aucun personnage trouvé");
            return new List<CharacterDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de la récupération des personnages");
            return new List<CharacterDto>();
        }
    }

    /// <summary>
    /// Récupère un personnage spécifique par son ID
    /// </summary>
    public async Task<CharacterDto?> GetCharacterAsync(int id)
    {
        try
        {
            _logger.LogInformation("🔍 Récupération du personnage {CharacterId}", id);

            var character = await GetAsync<CharacterDto>($"/api/characters/{id}");
            
            if (character != null)
            {
                _logger.LogInformation("✅ Personnage {CharacterName} récupéré", character.Name);
                return character;
            }
            
            _logger.LogWarning("⚠️ Personnage {CharacterId} non trouvé", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de la récupération du personnage {CharacterId}", id);
            return null;
        }
    }

    /// <summary>
    /// Crée un nouveau personnage
    /// </summary>
    public async Task<CharacterDto?> CreateCharacterAsync(CreateCharacterRequest request)
    {
        try
        {
            _logger.LogInformation("➕ Création du personnage {CharacterName}", request.Name);

            var character = await PostAsync<CreateCharacterRequest, CharacterDto>("/api/characters", request);
            
            if (character != null)
            {
                _logger.LogInformation("✅ Personnage {CharacterName} créé avec l'ID {CharacterId}", 
                    character.Name, character.Id);
                return character;
            }
            
            _logger.LogWarning("⚠️ Échec de création du personnage {CharacterName}", request.Name);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de la création du personnage {CharacterName}", request.Name);
            return null;
        }
    }

    /// <summary>
    /// Met à jour un personnage existant
    /// </summary>
    public async Task<CharacterDto?> UpdateCharacterAsync(int id, UpdateCharacterRequest request)
    {
        try
        {
            _logger.LogInformation("✏️ Mise à jour du personnage {CharacterId}", id);

            var character = await PutAsync<UpdateCharacterRequest, CharacterDto>($"/api/characters/{id}", request);
            
            if (character != null)
            {
                _logger.LogInformation("✅ Personnage {CharacterName} mis à jour", character.Name);
                return character;
            }
            
            _logger.LogWarning("⚠️ Échec de mise à jour du personnage {CharacterId}", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de la mise à jour du personnage {CharacterId}", id);
            return null;
        }
    }

    /// <summary>
    /// Supprime un personnage
    /// </summary>
    public async Task<bool> DeleteCharacterAsync(int id)
    {
        try
        {
            _logger.LogInformation("🗑️ Suppression du personnage {CharacterId}", id);

            var success = await DeleteAsync($"/api/characters/{id}");
            
            if (success)
            {
                _logger.LogInformation("✅ Personnage {CharacterId} supprimé", id);
                return true;
            }
            
            _logger.LogWarning("⚠️ Échec de suppression du personnage {CharacterId}", id);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors de la suppression du personnage {CharacterId}", id);
            return false;
        }
    }
}