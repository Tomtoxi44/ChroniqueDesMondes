namespace Cdm.Business.Common.Models.Campaign;

public class ChapterRequest
{
    public int CampaignId { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Notes { get; set; }
}

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
}

public class ChapterUpdateRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int? Order { get; set; }
    public bool? IsActive { get; set; }
    public string? Notes { get; set; }
}