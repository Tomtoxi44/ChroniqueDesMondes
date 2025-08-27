using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cdm.Data.Models;

[Table("CampaignParticipants")]
public class CampaignParticipant
{
    public int Id { get; set; }

    [Required]
    public int CampaignId { get; set; }

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; } = null!;

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;

    [Required]
    public ParticipantRole Role { get; set; } = ParticipantRole.Player;

    /// <summary>
    /// Permissions spécifiques du participant
    /// </summary>
    public ParticipantPermissions Permissions { get; set; } = ParticipantPermissions.ReadOnly;

    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Lien vers l'invitation qui a permis de rejoindre (optionnel)
    /// </summary>
    public int? InvitationId { get; set; }

    [ForeignKey(nameof(InvitationId))]
    public virtual CampaignInvitation? Invitation { get; set; }
}