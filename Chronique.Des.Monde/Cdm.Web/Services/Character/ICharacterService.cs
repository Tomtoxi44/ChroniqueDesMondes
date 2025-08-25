using Cdm.Web.Models;

namespace Cdm.Web.Services.Character;

public interface ICharacterService
{
    Task<ApiResponse<List<Models.Character>>> GetCharactersAsync(int userId);
    Task<ApiResponse<Models.Character>> GetCharacterAsync(int characterId);
    Task<ApiResponse<Models.Character>> CreateCharacterAsync(CharacterRequest request, int userId);
    Task<ApiResponse<Models.Character>> CreateDndCharacterAsync(CharacterDndRequest request, int userId);
}