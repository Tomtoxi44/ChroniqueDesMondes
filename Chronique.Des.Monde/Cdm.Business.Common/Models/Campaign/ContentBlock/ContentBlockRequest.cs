namespace Cdm.Business.Common.Models.Campaign.ContentBlock;

public class ContentBlockRequest
{
    public int ChapterId { get; set; }
    public int Order { get; set; }
    public string Type { get; set; } = string.Empty; // Location, NpcDialogue, Description, Event
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    // Pour les dialogues NPC/Character
    public int? CharacterId { get; set; }
    public string? NpcMood { get; set; } // Hostile, Neutral, Friendly, Scared, Angry
    
    // Tags pour organisation
    public List<string>? Tags { get; set; }
}