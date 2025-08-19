namespace Cmd.Abstraction;

using Cmd.Abstraction.ModelsRequest;
using Cmd.Abstraction.ModelsView;

public interface ICharacterBusiness
{
    Task<IEnumerable<ICharacterView>> GetAllCharactersByUserId(int userId);
    Task<ICharacterView> GetCharacterById(int characterId);
    Task<ICharacterView> CreateCharacter(CharacterRequest character, int userId);
    Task<ICharacterView> UpdateCharacter(CharacterRequest character, int characterId);
    Task DeleteCharacter(int characterId);
}
