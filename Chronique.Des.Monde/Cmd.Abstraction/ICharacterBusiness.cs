namespace Cmd.Abstraction;

using Cmd.Abstraction.ModelsRequest;
using Cmd.Abstraction.ModelsView;

public interface ICharacterBusiness
{
    ICharacterView GetCharacterByCharacterId(int characterId);

    void CreateCharacter(CharacterRequest character);
}
