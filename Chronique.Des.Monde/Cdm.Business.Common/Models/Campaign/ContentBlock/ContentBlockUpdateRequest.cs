namespace Cdm.Business.Common.Models.Campaign.ContentBlock;

public class ContentBlockUpdateRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int? Order { get; set; }
    public int? CharacterId { get; set; }
    public string? NpcMood { get; set; }
    public List<string>? Tags { get; set; }
    public bool? IsActive { get; set; }
}