using Cdm.Common.Enums;

namespace Cdm.Business.Common.Models.Equipment;

/// <summary>
/// Modèle de requête pour créer un équipement générique
/// </summary>
public record EquipmentRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public GameType GameType { get; init; } = GameType.Generic;
    public List<string>? Tags { get; init; }
    public string Category { get; init; } = string.Empty;
    public decimal Weight { get; init; } = 0;
    public decimal Value { get; init; } = 0;
}