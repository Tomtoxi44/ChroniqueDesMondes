namespace Cmd.Abstraction.ModelsRequest;

using Cmd.Abstraction.ModelsView;

public class CharacterRequest
{
    public string? Background { get; set; }
    public int Id { get; set; }
    public int Leveling { get; set; }
    public int Life { get; set; }
    public string? Name { get; set; }
    public string? Picture { get; set; }

    public IReadOnlyDictionary<string, object> Competences { get; }

    public IReadOnlyDictionary<string, object> Stats { get; }
}
