using Cdm.Common.Enums;

namespace Cdm.Business.Common.Models.Campaign.Npc;

public class NpcRequest
{
    public int ChapterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GameType GameType { get; set; } = GameType.Generic;
    public bool IsHostile { get; set; } = false;
    
    // Tags pour organisation et intégration combat
    public List<string>? Tags { get; set; } // ["monster", "humanoid", "beast", "official-dnd"]
    
    // Propriétés spécialisées (JSON)
    public string? DndProperties { get; set; }
    public string? GenericProperties { get; set; }
}