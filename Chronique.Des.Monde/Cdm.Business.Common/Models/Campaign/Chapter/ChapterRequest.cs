namespace Cdm.Business.Common.Models.Campaign.Chapter;

public class ChapterRequest
{
    public int CampaignId { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Notes { get; set; }
}