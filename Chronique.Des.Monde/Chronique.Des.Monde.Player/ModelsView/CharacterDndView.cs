namespace Cmd.Business.Character.Models;

using Cmd.Abstraction.ModelsView;

public class CharacterDndView : ICharacterView
{
    public int Id { get; set; }

    public string Picture { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Background { get; set; } = string.Empty;

    public string Class { get; set; } = string.Empty;

    public int Life { get; set; }

    public int Leveling { get; set; }

    public int ClassArmor { get; set; }

    public int Strong { get; set; }

    public int AdditionalStrong { get; set; }

    public int Dexterity { get; set; }

    public int AdditionalDexterity { get; set; }

    public int Constitution { get; set; }

    public int AdditionalConstitution { get; set; }

    public int Intelligence { get; set; }

    public int AdditionalIntelligence { get; set; }

    public int Wisdoms { get; set; }

    public int AdditionalWisdoms { get; set; }

    public int Charism { get; set; }

    public int AdditionalCharism { get; set; }

    public IReadOnlyDictionary<string, object> Competences => new Dictionary<string, object>()
    {
        { nameof(Dexterity), this.Dexterity},
        { nameof(Intelligence), this.Intelligence},
    };

    public IReadOnlyDictionary<string, object> Stats => new Dictionary<string, object>();
}
