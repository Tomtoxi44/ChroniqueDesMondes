namespace Cmd.Abstraction.ModelsView;

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
