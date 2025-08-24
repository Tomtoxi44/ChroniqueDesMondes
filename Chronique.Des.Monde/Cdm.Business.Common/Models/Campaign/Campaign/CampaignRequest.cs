using Cdm.Common.Enums;

namespace Cdm.Business.Common.Models.Campaign.Campaign;

public class CampaignRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GameType GameType { get; set; } = GameType.Generic;
    public bool IsPublic { get; set; } = false;
    public string? Settings { get; set; }
}