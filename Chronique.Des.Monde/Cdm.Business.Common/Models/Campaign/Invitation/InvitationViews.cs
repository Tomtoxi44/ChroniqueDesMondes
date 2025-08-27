using Cdm.Data.Models;

namespace Cdm.Business.Common.Models.Campaign.Invitation;

public class InvitationView
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public InvitationStatus Status { get; set; }
    public string Token { get; set; } = string.Empty;
    public string InvitedByName { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public bool IsExpired => DateTime.UtcNow > ExpiresAt && Status == InvitationStatus.Pending;
}

public class ParticipantView
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public ParticipantRole Role { get; set; }
    public ParticipantPermissions Permissions { get; set; }
    public DateTime JoinedDate { get; set; }
    public bool IsActive { get; set; }
    public int? InvitationId { get; set; }
}

public class CampaignMembersView
{
    public int CampaignId { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public string GameMasterName { get; set; } = string.Empty;
    public List<ParticipantView> Participants { get; set; } = new();
    public List<InvitationView> PendingInvitations { get; set; } = new();
}