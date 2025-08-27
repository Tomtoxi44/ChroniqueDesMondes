using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cdm.Data.Models;

[Table("CampaignInvitations")]
public class CampaignInvitation
{
    public int Id { get; set; }

    [Required]
    public int CampaignId { get; set; }

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Si l'utilisateur existe déjà, on peut référencer directement
    /// </summary>
    public int? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }

    [Required]
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;

    /// <summary>
    /// Token unique pour valider l'invitation via lien email
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Qui a envoyé l'invitation
    /// </summary>
    [Required]
    public int InvitedById { get; set; }

    [ForeignKey(nameof(InvitedById))]
    public virtual User InvitedBy { get; set; } = null!;

    [MaxLength(500)]
    public string? Message { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RespondedAt { get; set; }
}