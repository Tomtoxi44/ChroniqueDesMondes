namespace Cdm.Business.Common.Models.Equipment;

/// <summary>
/// Modèle de requête pour mettre à jour un équipement générique
/// </summary>
public record EquipmentUpdateRequest
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public List<string>? Tags { get; init; }
    public string? Category { get; init; }
    public decimal? Weight { get; init; }
    public decimal? Value { get; init; }
}