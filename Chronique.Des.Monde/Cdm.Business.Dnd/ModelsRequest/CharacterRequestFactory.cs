namespace Cdm.Business.Dnd.ModelsRequest;

using Cmd.Abstraction.ModelsRequest;

public static class CharacterRequestFactory
{
    /// <summary>
    /// Crée un CharacterRequest pour D&D avec les compétences spécifiques
    /// </summary>
    public static CharacterRequest CreateDndCharacterRequest(
        string name,
        int leveling,
        int life,
        string picture = "",
        string? background = null,
        string characterClass = "",
        int classArmor = 10,
        int strong = 10,
        int dexterity = 10,
        int constitution = 10,
        int intelligence = 10,
        int wisdoms = 10,
        int charism = 10)
    {
        var competences = new Dictionary<string, object>
        {
            { "Class", characterClass },
            { "ClassArmor", classArmor },
            { "Strong", strong },
            { "Dexterity", dexterity },
            { "Constitution", constitution },
            { "Intelligence", intelligence },
            { "Wisdoms", wisdoms },
            { "Charism", charism }
        };

        return new CharacterRequest
        {
            Name = name,
            Leveling = leveling,
            Life = life,
            Picture = picture,
            Background = background,
            Competences = competences,
            Stats = new Dictionary<string, object>() // Les stats additionnelles seront calculées automatiquement
        };
    }
}