using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cdm.Data.Models;

namespace Cdm.Data.Common.Models;

/// <summary>
/// Proposition d'équipement d'un MJ vers un joueur
/// </summary>
public class EquipmentOffer
{
    public int Id { get; set; }

    [Required]
    public int CampaignId { get; set; }

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; } = null!;

    [Required]
    public int GameMasterId { get; set; } // UserId du MJ

    [ForeignKey(nameof(GameMasterId))]
    public virtual User GameMaster { get; set; } = null!;

    [Required]
    public int TargetPlayerId { get; set; } // UserId du joueur ciblé

    [ForeignKey(nameof(TargetPlayerId))]
    public virtual User TargetPlayer { get; set; } = null!;

    [Required]
    public int EquipmentId { get; set; } // ID de l'équipement proposé

    [ForeignKey(nameof(EquipmentId))]
    public virtual AEquipment Equipment { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    [MaxLength(500)]
    public string? Message { get; set; } // Message du MJ

    [Required]
    public OfferStatus Status { get; set; } = OfferStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? RespondedAt { get; set; }

    [MaxLength(500)]
    public string? ResponseMessage { get; set; } // Réponse du joueur
}

/// <summary>
/// Échange d'équipement entre deux joueurs
/// </summary>
public class EquipmentTrade
{
    public int Id { get; set; }

    [Required]
    public int CampaignId { get; set; }

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; } = null!;

    [Required]
    public int FromPlayerId { get; set; } // Joueur qui donne

    [ForeignKey(nameof(FromPlayerId))]
    public virtual User FromPlayer { get; set; } = null!;

    [Required]
    public int ToPlayerId { get; set; } // Joueur qui reçoit

    [ForeignKey(nameof(ToPlayerId))]
    public virtual User ToPlayer { get; set; } = null!;

    [Required]
    public int EquipmentId { get; set; } // Équipement échangé

    [ForeignKey(nameof(EquipmentId))]
    public virtual AEquipment Equipment { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    [MaxLength(500)]
    public string? Message { get; set; } // Message d'accompagnement

    [Required]
    public TradeStatus Status { get; set; } = TradeStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// Inventaire d'un personnage (liaison entre personnage et équipements)
/// </summary>
public class CharacterInventory
{
    public int Id { get; set; }

    [Required]
    public int CharacterId { get; set; }

    [ForeignKey(nameof(CharacterId))]
    public virtual ACharacter Character { get; set; } = null!;

    [Required]
    public int EquipmentId { get; set; }

    [ForeignKey(nameof(EquipmentId))]
    public virtual AEquipment Equipment { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    public bool IsEquipped { get; set; } = false; // Si l'équipement est actuellement équipé

    public DateTime ObtainedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Notes { get; set; } // Notes personnelles du joueur
}

/// <summary>
/// Statut d'une proposition d'équipement
/// </summary>
public enum OfferStatus
{
    Pending,   // En attente de réponse
    Accepted,  // Acceptée
    Declined,  // Refusée
    Expired    // Expirée
}

/// <summary>
/// Statut d'un échange d'équipement
/// </summary>
public enum TradeStatus
{
    Pending,   // En attente d'acceptation
    Accepted,  // Accepté et en cours
    Completed, // Terminé avec succès
    Cancelled, // Annulé
    Failed     // Échec (ex: quantité insuffisante)
}