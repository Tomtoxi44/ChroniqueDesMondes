using Cdm.Common.Enums;

namespace Cdm.Business.Common.Models.Spells;

/// <summary>
/// Modèle de requête pour créer un sort générique
/// </summary>
public record SpellRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public GameType GameType { get; init; } = GameType.Generic;
    public List<string>? Tags { get; init; }
}