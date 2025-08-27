using System.ComponentModel.DataAnnotations;

namespace Cdm.Business.Common.Models.Campaign.Invitation;

public class InvitationRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Message { get; set; }

    /// <summary>
    /// Durée de validité en jours (par défaut 7 jours)
    /// </summary>
    public int ExpirationDays { get; set; } = 7;
}

public class InvitationResponse
{
    [Required]
    public bool Accept { get; set; }

    [MaxLength(500)]
    public string? Message { get; set; }
}