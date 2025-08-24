using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chronique.Des.Mondes.Data.Models;

[Table("Campaigns")]
public class Campaign
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// GameType stored as int (0=Generic, 1=DnD, 2=Skyrim)
    /// </summary>
    [Required]
    public int GameType { get; set; } = 0; // Generic by default

    public bool IsPublic { get; set; } = false;

    [Required]
    public int CreatedById { get; set; }

    [ForeignKey(nameof(CreatedById))]
    public virtual User CreatedBy { get; set; } = null!;

    /// <summary>
    /// JSON settings specific to the GameType for campaign configuration
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? Settings { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
}