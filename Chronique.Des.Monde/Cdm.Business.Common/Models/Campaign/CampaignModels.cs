using Cdm.Common.Enums;

namespace Cdm.Business.Common.Models.Campaign;

public class CampaignRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GameType GameType { get; set; } = GameType.Generic;
    public bool IsPublic { get; set; } = false;
    public string? Settings { get; set; }
}

public class CampaignView
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GameType GameType { get; set; }
    public bool IsPublic { get; set; }
    public int CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string? Settings { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsActive { get; set; }
    public int ChapterCount { get; set; }
}

public class CampaignDetailView : CampaignView
{
    public List<ChapterView> Chapters { get; set; } = new List<ChapterView>();
}