using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chronique.Des.Mondes.Data.Models;

[Table("ContentBlocks")]
public class ContentBlock
{
    public int Id { get; set; }

    [Required]
    public int ChapterId { get; set; }

    [ForeignKey(nameof(ChapterId))]
    public virtual Chapter Chapter { get; set; } = null!;

    [Required]
    public int Order { get; set; }

    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = string.Empty; // Location, NpcDialogue, Description, Event

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Content { get; set; } = string.Empty; // Rich content (HTML/Markdown)

    // Liaison optionnelle à un Character/NPC (pour les dialogues)
    public int? CharacterId { get; set; }

    [ForeignKey(nameof(CharacterId))]
    public virtual ACharacter? Character { get; set; }

    [MaxLength(20)]
    public string? NpcMood { get; set; } // Hostile, Neutral, Friendly, Scared, Angry

    // Tags pour organisation (JSON)
    [Column(TypeName = "nvarchar(max)")]
    public string? Tags { get; set; } // JSON array: ["combat", "puzzle", "social", "exploration"]

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }
}