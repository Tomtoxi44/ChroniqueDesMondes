namespace Cdm.Business.Common.Models.Spells;

/// <summary>
/// Modèle de requête pour mettre à jour un sort générique
/// </summary>
public record SpellUpdateRequest
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public List<string>? Tags { get; init; }
}