namespace Cmd.Abstraction.Equipment;

/// <summary>
/// Interface pour les services d'échange d'équipements
/// Utilise des DTOs simples pour éviter les dépendances circulaires
/// </summary>
public interface IEquipmentExchangeService
{
    // Propositions MJ → Joueur
    Task<int> CreateOfferAsync(int campaignId, int gmId, int playerId, int equipmentId, int quantity, string? message);
    Task<bool> RespondToOfferAsync(int offerId, int playerId, bool accepted, string? responseMessage);
    Task<IEnumerable<EquipmentOfferDto>> GetPlayerOffersAsync(int campaignId, int playerId);
    Task<IEnumerable<EquipmentOfferDto>> GetGmOffersAsync(int campaignId, int gmId);

    // Échanges Joueur ↔ Joueur
    Task<int> ProposeTradeAsync(int campaignId, int fromPlayerId, int toPlayerId, int equipmentId, int quantity, string? message);
    Task<bool> RespondToTradeAsync(int tradeId, int playerId, bool accepted);
    Task<IEnumerable<EquipmentTradeDto>> GetPlayerTradesAsync(int campaignId, int playerId);

    // Inventaire
    Task<IEnumerable<InventoryItemDto>> GetCharacterInventoryAsync(int characterId);
    Task<int> AddEquipmentToInventoryAsync(int characterId, int equipmentId, int quantity, string? notes);
    Task RemoveEquipmentFromInventoryAsync(int characterId, int equipmentId, int quantity);
    Task<bool> EquipItemAsync(int characterId, int equipmentId, bool equipped);

    // Validation
    Task<bool> ArePlayersInSameCampaignAsync(int campaignId, int playerId1, int playerId2);
    Task<bool> IsPlayerInCampaignAsync(int campaignId, int playerId);
    Task<bool> HasSufficientQuantityAsync(int characterId, int equipmentId, int requiredQuantity);
}

/// <summary>
/// DTO pour les propositions d'équipement
/// </summary>
public record EquipmentOfferDto
{
    public int Id { get; init; }
    public int CampaignId { get; init; }
    public int GameMasterId { get; init; }
    public string GameMasterName { get; init; } = string.Empty;
    public int TargetPlayerId { get; init; }
    public string TargetPlayerName { get; init; } = string.Empty;
    public int EquipmentId { get; init; }
    public string EquipmentName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public string? Message { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? RespondedAt { get; init; }
    public string? ResponseMessage { get; init; }
}

/// <summary>
/// DTO pour les échanges d'équipement
/// </summary>
public record EquipmentTradeDto
{
    public int Id { get; init; }
    public int CampaignId { get; init; }
    public int FromPlayerId { get; init; }
    public string FromPlayerName { get; init; } = string.Empty;
    public int ToPlayerId { get; init; }
    public string ToPlayerName { get; init; } = string.Empty;
    public int EquipmentId { get; init; }
    public string EquipmentName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public string? Message { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
}

/// <summary>
/// DTO pour les objets d'inventaire
/// </summary>
public record InventoryItemDto
{
    public int Id { get; init; }
    public int CharacterId { get; init; }
    public int EquipmentId { get; init; }
    public string EquipmentName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public bool IsEquipped { get; init; }
    public DateTime ObtainedAt { get; init; }
    public string? Notes { get; init; }
}