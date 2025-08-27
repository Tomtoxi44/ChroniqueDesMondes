namespace Cdm.Business.Dnd.ModelsView;

using Cmd.Abstraction.ModelsView;

public class GetAllCharacterDndRequestView
{
    public int UserId { get; set; }
    public string? ClassFilter { get; set; }
    public int? MinLevel { get; set; }
    public int? MaxLevel { get; set; }
}