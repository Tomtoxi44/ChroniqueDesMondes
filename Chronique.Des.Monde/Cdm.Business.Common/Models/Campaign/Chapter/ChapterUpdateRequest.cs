namespace Cdm.Business.Common.Models.Campaign.Chapter;

public class ChapterUpdateRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int? Order { get; set; }
    public bool? IsActive { get; set; }
    public string? Notes { get; set; }
}