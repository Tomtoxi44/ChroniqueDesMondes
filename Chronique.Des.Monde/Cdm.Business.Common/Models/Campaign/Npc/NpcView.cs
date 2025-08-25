using Cdm.Common.Enums;

namespace Cdm.Business.Common.Models.Campaign.Npc;

public class NpcView
{
    public int Id { get; set; }
    public int ChapterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GameType GameType { get; set; }
    public bool IsHostile { get; set; }
    
    // Tags pour organisation et intégration combat
    public List<string> Tags { get; set; } = new List<string>();
    
    // Propriétés spécialisées
    public string? DndProperties { get; set; }
    public string? GenericProperties { get; set; }
    
    // Indique si c'est un NPC système (fourni par défaut) ou créé par l'utilisateur
    public bool IsSystemCharacter { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Nombre de blocs de dialogue liés
    public int DialogueBlocksCount { get; set; }
}