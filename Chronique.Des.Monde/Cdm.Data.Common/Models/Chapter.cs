using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cdm.Data.Models;

[Table("Chapters")]
public class Chapter
{
    public int Id { get; set; }

    [Required]
    public int CampaignId { get; set; }

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; } = null!;

    [Required]
    public int Order { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Column(TypeName = "nvarchar(max)")]
    public string? Content { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Optional notes or metadata specific to this chapter
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Notes { get; set; } // Notes privées du MJ

    // Navigation properties
    public virtual ICollection<ContentBlock> ContentBlocks { get; set; } = new List<ContentBlock>();
    public virtual ICollection<ACharacter> Characters { get; set; } = new List<ACharacter>(); // NPCs du chapitre
}