using Cdm.Common.Enums;

namespace Cdm.Business.Common.Models.Equipment;

/// <summary>
/// Modèle de vue pour afficher un équipement (générique)
/// </summary>
public record EquipmentView
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public GameType GameType { get; init; }
    public bool IsPublic { get; init; }
    public string Source { get; init; } = string.Empty; // "Official" ou "Private"
    public string? CreatedByName { get; init; }
    public List<string> Tags { get; init; } = new();
    public string Category { get; init; } = string.Empty;
    public decimal Weight { get; init; }
    public decimal Value { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}