namespace Cmd.Abstraction;

public interface ICharacterBusiness
{
    ICharacterView GetCharacterByPlayerId(int characterId);

    void CreateCharacter(CharacterRequest character);
}

public class CharacterRequest : ICharacterView
{
    public string? Background { get; set; }
    public int Id { get; set; }
    public int Leveling { get; set; }
    public int Life { get; set; }
    public string Name { get; set; }
    public string Picture { get; set; }

    public IReadOnlyDictionary<string, object> Competences { get; }

    public IReadOnlyDictionary<string, object> Stats { get; }
}

public interface ICharacterView
{
    string? Background { get; set; }
    int Id { get; set; }
    int Leveling { get; set; }
    int Life { get; set; }
    string Name { get; set; }
    string Picture { get; set; }

    IReadOnlyDictionary<string, object> Competences { get; }

    IReadOnlyDictionary<string, object> Stats { get; }
}
