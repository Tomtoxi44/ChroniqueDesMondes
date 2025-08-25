using Cdm.Business.Common.Models.Campaign.Npc;
using ContentBlock = Cdm.Business.Common.Models.Campaign.ContentBlock;

namespace Cdm.Business.Common.Models.Campaign.Chapter;

public class ChapterDetailView : ChapterView
{
    public List<ContentBlock.ContentBlockView> ContentBlocks { get; set; } = new List<ContentBlock.ContentBlockView>();
    public List<NpcView> Npcs { get; set; } = new List<NpcView>();
}