namespace Cdm.Business.Common.Models.Campaign.Chapter;

public class ChapterView
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Notes { get; set; }
    
    // Compteurs
    public int ContentBlocksCount { get; set; }
    public int CharactersCount { get; set; } // NPCs total
    public int HostileCharactersCount { get; set; } // NPCs hostiles
}