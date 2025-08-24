namespace Cdm.Business.Common.Models.Campaign.Npc;

public class NpcUpdateRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsHostile { get; set; }
    public List<string>? Tags { get; set; }
    public string? DndProperties { get; set; }
    public string? GenericProperties { get; set; }
}