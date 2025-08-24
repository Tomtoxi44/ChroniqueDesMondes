namespace Cdm.Business.Common.Models.Campaign.ContentBlock;

public class ContentBlockView
{
    public int Id { get; set; }
    public int ChapterId { get; set; }
    public int Order { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    // Character/NPC info (si applicable)
    public int? CharacterId { get; set; }
    public string? CharacterName { get; set; }
    public string? NpcMood { get; set; }
    
    // Tags
    public List<string> Tags { get; set; } = new List<string>();
    
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}