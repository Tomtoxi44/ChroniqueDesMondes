using Cdm.Data.Common.Models;
using Cdm.Data.Dnd;
using Cdm.Common;
using Cmd.Abstraction.Equipment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cdm.Business.Common.Business.Equipment;

/// <summary>
/// Service pour la gestion des échanges d'équipements
/// </summary>
public class EquipmentExchangeService : IEquipmentExchangeService
{
    private readonly DndDbContext context;
    private readonly ILogger<EquipmentExchangeService> logger;

    public EquipmentExchangeService(DndDbContext context, ILogger<EquipmentExchangeService> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    // === PROPOSITIONS MJ → JOUEUR ===

    public async Task<int> CreateOfferAsync(int campaignId, int gmId, int playerId, int equipmentId, int quantity, string? message)
    {
        this.logger.LogInformation("GM {GmId} creating equipment offer for player {PlayerId} in campaign {CampaignId}", 
            gmId, playerId, campaignId);

        // Vérifications de base
        if (!await this.IsPlayerInCampaignAsync(campaignId, gmId))
        {
            throw new BusinessException("Vous n'êtes pas membre de cette campagne.");
        }

        if (!await this.IsPlayerInCampaignAsync(campaignId, playerId))
        {
            throw new BusinessException("Le joueur ciblé n'est pas membre de cette campagne.");
        }

        // Vérifier que l'équipement existe
        var equipment = await this.context.EquipmentDnd.FindAsync(equipmentId);
        if (equipment == null)
        {
            throw new BusinessException("Équipement introuvable.");
        }

        // Créer la proposition
        var offer = new EquipmentOffer
        {
            CampaignId = campaignId,
            GameMasterId = gmId,
            TargetPlayerId = playerId,
            EquipmentId = equipmentId,
            Quantity = quantity,
            Message = message,
            Status = OfferStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        this.context.EquipmentOffers.Add(offer);
        await this.context.SaveChangesAsync();

        this.logger.LogInformation("Equipment offer {OfferId} created successfully", offer.Id);
        return offer.Id;
    }

    public async Task<bool> RespondToOfferAsync(int offerId, int playerId, bool accepted, string? responseMessage)
    {
        this.logger.LogInformation("Player {PlayerId} responding to offer {OfferId}: {Accepted}", 
            playerId, offerId, accepted);

        var offer = await this.context.EquipmentOffers
            .Include(o => o.Equipment)
            .FirstOrDefaultAsync(o => o.Id == offerId);

        if (offer == null || offer.TargetPlayerId != playerId)
        {
            return false;
        }

        if (offer.Status != OfferStatus.Pending)
        {
            throw new BusinessException("Cette proposition a déjà reçu une réponse.");
        }

        offer.Status = accepted ? OfferStatus.Accepted : OfferStatus.Declined;
        offer.RespondedAt = DateTime.UtcNow;
        offer.ResponseMessage = responseMessage;

        // Si accepté, ajouter l'équipement à l'inventaire du joueur
        if (accepted)
        {
            // Trouver un personnage du joueur dans cette campagne pour ajouter l'équipement
            var playerCharacter = await this.context.CharactersDnd
                .FirstOrDefaultAsync(c => c.UserId == playerId); // TODO: Filtrer par campagne

            if (playerCharacter != null)
            {
                await this.AddEquipmentToInventoryAsync(playerCharacter.Id, offer.EquipmentId, offer.Quantity, 
                    $"Reçu du MJ: {offer.Message}");
            }
        }

        await this.context.SaveChangesAsync();

        this.logger.LogInformation("Offer {OfferId} response processed: {Status}", offerId, offer.Status);
        return true;
    }

    public async Task<IEnumerable<EquipmentOfferDto>> GetPlayerOffersAsync(int campaignId, int playerId)
    {
        var offers = await this.context.EquipmentOffers
            .Include(o => o.GameMaster)
            .Include(o => o.Equipment)
            .Where(o => o.CampaignId == campaignId && o.TargetPlayerId == playerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return offers.Select(o => new EquipmentOfferDto
        {
            Id = o.Id,
            CampaignId = o.CampaignId,
            GameMasterId = o.GameMasterId,
            GameMasterName = o.GameMaster?.UserName ?? "",
            TargetPlayerId = o.TargetPlayerId,
            TargetPlayerName = "", // TODO: Récupérer le nom du joueur
            EquipmentId = o.EquipmentId,
            EquipmentName = o.Equipment?.Name ?? "",
            Quantity = o.Quantity,
            Message = o.Message,
            Status = o.Status.ToString(),
            CreatedAt = o.CreatedAt,
            RespondedAt = o.RespondedAt,
            ResponseMessage = o.ResponseMessage
        });
    }

    public async Task<IEnumerable<EquipmentOfferDto>> GetGmOffersAsync(int campaignId, int gmId)
    {
        var offers = await this.context.EquipmentOffers
            .Include(o => o.TargetPlayer)
            .Include(o => o.Equipment)
            .Where(o => o.CampaignId == campaignId && o.GameMasterId == gmId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return offers.Select(o => new EquipmentOfferDto
        {
            Id = o.Id,
            CampaignId = o.CampaignId,
            GameMasterId = o.GameMasterId,
            GameMasterName = "",
            TargetPlayerId = o.TargetPlayerId,
            TargetPlayerName = o.TargetPlayer?.UserName ?? "",
            EquipmentId = o.EquipmentId,
            EquipmentName = o.Equipment?.Name ?? "",
            Quantity = o.Quantity,
            Message = o.Message,
            Status = o.Status.ToString(),
            CreatedAt = o.CreatedAt,
            RespondedAt = o.RespondedAt,
            ResponseMessage = o.ResponseMessage
        });
    }

    // === ÉCHANGES JOUEUR ↔ JOUEUR ===

    public async Task<int> ProposeTradeAsync(int campaignId, int fromPlayerId, int toPlayerId, int equipmentId, int quantity, string? message)
    {
        this.logger.LogInformation("Player {FromPlayerId} proposing trade to player {ToPlayerId} in campaign {CampaignId}", 
            fromPlayerId, toPlayerId, campaignId);

        // Vérifications
        if (!await this.ArePlayersInSameCampaignAsync(campaignId, fromPlayerId, toPlayerId))
        {
            throw new BusinessException("Les joueurs ne sont pas dans la même campagne.");
        }

        // Créer l'échange
        var trade = new EquipmentTrade
        {
            CampaignId = campaignId,
            FromPlayerId = fromPlayerId,
            ToPlayerId = toPlayerId,
            EquipmentId = equipmentId,
            Quantity = quantity,
            Message = message,
            Status = TradeStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        this.context.EquipmentTrades.Add(trade);
        await this.context.SaveChangesAsync();

        return trade.Id;
    }

    public async Task<bool> RespondToTradeAsync(int tradeId, int playerId, bool accepted)
    {
        var trade = await this.context.EquipmentTrades
            .Include(t => t.Equipment)
            .FirstOrDefaultAsync(t => t.Id == tradeId);

        if (trade == null || trade.ToPlayerId != playerId)
        {
            return false;
        }

        if (trade.Status != TradeStatus.Pending)
        {
            throw new BusinessException("Cet échange a déjà reçu une réponse.");
        }

        if (accepted)
        {
            // Transférer l'équipement
            // 1. Retirer de l'inventaire du donneur
            var fromCharacter = await this.context.CharactersDnd
                .FirstOrDefaultAsync(c => c.UserId == trade.FromPlayerId);
            
            var toCharacter = await this.context.CharactersDnd
                .FirstOrDefaultAsync(c => c.UserId == trade.ToPlayerId);

            if (fromCharacter != null && toCharacter != null)
            {
                // Vérifier la quantité disponible
                if (!await this.HasSufficientQuantityAsync(fromCharacter.Id, trade.EquipmentId, trade.Quantity))
                {
                    trade.Status = TradeStatus.Failed;
                    await this.context.SaveChangesAsync();
                    throw new BusinessException("Quantité insuffisante dans l'inventaire.");
                }

                // Effectuer le transfert
                await this.RemoveEquipmentFromInventoryAsync(fromCharacter.Id, trade.EquipmentId, trade.Quantity);
                await this.AddEquipmentToInventoryAsync(toCharacter.Id, trade.EquipmentId, trade.Quantity, 
                    $"Échange avec {fromCharacter.Name}: {trade.Message}");

                trade.Status = TradeStatus.Completed;
            }
            else
            {
                trade.Status = TradeStatus.Failed;
            }
        }
        else
        {
            trade.Status = TradeStatus.Cancelled;
        }

        trade.CompletedAt = DateTime.UtcNow;
        await this.context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<EquipmentTradeDto>> GetPlayerTradesAsync(int campaignId, int playerId)
    {
        var trades = await this.context.EquipmentTrades
            .Include(t => t.FromPlayer)
            .Include(t => t.ToPlayer)
            .Include(t => t.Equipment)
            .Where(t => t.CampaignId == campaignId && 
                       (t.FromPlayerId == playerId || t.ToPlayerId == playerId))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return trades.Select(t => new EquipmentTradeDto
        {
            Id = t.Id,
            CampaignId = t.CampaignId,
            FromPlayerId = t.FromPlayerId,
            FromPlayerName = t.FromPlayer?.UserName ?? "",
            ToPlayerId = t.ToPlayerId,
            ToPlayerName = t.ToPlayer?.UserName ?? "",
            EquipmentId = t.EquipmentId,
            EquipmentName = t.Equipment?.Name ?? "",
            Quantity = t.Quantity,
            Message = t.Message,
            Status = t.Status.ToString(),
            CreatedAt = t.CreatedAt,
            CompletedAt = t.CompletedAt
        });
    }

    // === INVENTAIRE ===

    public async Task<IEnumerable<InventoryItemDto>> GetCharacterInventoryAsync(int characterId)
    {
        var inventory = await this.context.CharacterInventory
            .Include(i => i.Equipment)
            .Where(i => i.CharacterId == characterId)
            .OrderBy(i => i.Equipment.Category)
            .ThenBy(i => i.Equipment.Name)
            .ToListAsync();

        return inventory.Select(i => new InventoryItemDto
        {
            Id = i.Id,
            CharacterId = i.CharacterId,
            EquipmentId = i.EquipmentId,
            EquipmentName = i.Equipment?.Name ?? "",
            Category = i.Equipment?.Category ?? "",
            Quantity = i.Quantity,
            IsEquipped = i.IsEquipped,
            ObtainedAt = i.ObtainedAt,
            Notes = i.Notes
        });
    }

    public async Task<int> AddEquipmentToInventoryAsync(int characterId, int equipmentId, int quantity, string? notes)
    {
        // Vérifier si l'équipement existe déjà dans l'inventaire
        var existingItem = await this.context.CharacterInventory
            .FirstOrDefaultAsync(i => i.CharacterId == characterId && i.EquipmentId == equipmentId);

        if (existingItem != null)
        {
            // Augmenter la quantité
            existingItem.Quantity += quantity;
            await this.context.SaveChangesAsync();
            return existingItem.Id;
        }
        else
        {
            // Créer un nouvel élément d'inventaire
            var inventoryItem = new CharacterInventory
            {
                CharacterId = characterId,
                EquipmentId = equipmentId,
                Quantity = quantity,
                IsEquipped = false,
                ObtainedAt = DateTime.UtcNow,
                Notes = notes
            };

            this.context.CharacterInventory.Add(inventoryItem);
            await this.context.SaveChangesAsync();
            return inventoryItem.Id;
        }
    }

    public async Task RemoveEquipmentFromInventoryAsync(int characterId, int equipmentId, int quantity)
    {
        var inventoryItem = await this.context.CharacterInventory
            .FirstOrDefaultAsync(i => i.CharacterId == characterId && i.EquipmentId == equipmentId);

        if (inventoryItem == null)
        {
            throw new BusinessException("Cet équipement n'est pas dans l'inventaire.");
        }

        if (inventoryItem.Quantity < quantity)
        {
            throw new BusinessException("Quantité insuffisante dans l'inventaire.");
        }

        inventoryItem.Quantity -= quantity;

        if (inventoryItem.Quantity <= 0)
        {
            // Supprimer complètement l'élément si la quantité atteint 0
            this.context.CharacterInventory.Remove(inventoryItem);
        }

        await this.context.SaveChangesAsync();
    }

    public async Task<bool> EquipItemAsync(int characterId, int equipmentId, bool equipped)
    {
        var inventoryItem = await this.context.CharacterInventory
            .FirstOrDefaultAsync(i => i.CharacterId == characterId && i.EquipmentId == equipmentId);

        if (inventoryItem == null)
        {
            return false;
        }

        inventoryItem.IsEquipped = equipped;
        await this.context.SaveChangesAsync();

        return true;
    }

    // === VALIDATION ===

    public async Task<bool> ArePlayersInSameCampaignAsync(int campaignId, int playerId1, int playerId2)
    {
        var player1InCampaign = await this.IsPlayerInCampaignAsync(campaignId, playerId1);
        var player2InCampaign = await this.IsPlayerInCampaignAsync(campaignId, playerId2);

        return player1InCampaign && player2InCampaign;
    }

    public async Task<bool> IsPlayerInCampaignAsync(int campaignId, int playerId)
    {
        // TODO: Implémenter la vérification avec CampaignParticipants
        // Pour l'instant, on suppose que oui
        return true;
    }

    public async Task<bool> HasSufficientQuantityAsync(int characterId, int equipmentId, int requiredQuantity)
    {
        var inventoryItem = await this.context.CharacterInventory
            .FirstOrDefaultAsync(i => i.CharacterId == characterId && i.EquipmentId == equipmentId);

        return inventoryItem?.Quantity >= requiredQuantity;
    }
}