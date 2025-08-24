using Cdm.Business.Common.Models.Campaign.Chapter;

namespace Cdm.Business.Common.Models.Campaign.Campaign;

public class CampaignDetailView : CampaignView
{
    public List<ChapterView> Chapters { get; set; } = new List<ChapterView>();
}