using Cdm.Common.Enums;

namespace Cmd.Abstraction.Equipment;

/// <summary>
/// Interface abstraite pour les services d'équipements, suivant le pattern ISpellBusiness
/// </summary>
public interface IEquipmentBusiness
{
    Task<IEnumerable<IEquipmentView>> GetAllEquipmentsByUserId(int userId, GameType gameType);
    Task<IEquipmentView> GetEquipmentById(int equipmentId, int userId);
    Task<IEquipmentView> CreateEquipment(EquipmentRequest equipment, int userId);
    Task<IEquipmentView> UpdateEquipment(EquipmentRequest equipment, int equipmentId, int userId);
    Task DeleteEquipment(int equipmentId, int userId);
    Task<IEnumerable<IEquipmentView>> SearchEquipments(string searchText, int userId, GameType gameType);
    Task<IEnumerable<IEquipmentView>> GetEquipmentsByCategory(string category, int userId, GameType gameType);
}

/// <summary>
/// Interface pour les vues d'équipements
/// </summary>
public interface IEquipmentView
{
    int Id { get; }
    string Name { get; }
    string Description { get; }
    string? ImageUrl { get; }
    GameType GameType { get; }
    bool IsPublic { get; }
    string Source { get; }
    List<string> Tags { get; }
    string Category { get; }
    decimal Weight { get; }
    decimal Value { get; }
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
}

/// <summary>
/// Modèle de requête générique pour les équipements
/// </summary>
public record EquipmentRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public GameType GameType { get; init; } = GameType.Generic;
    public List<string>? Tags { get; init; }
    public string Category { get; init; } = string.Empty; // Ex: "Weapon", "Armor", "Tool"
    public decimal Weight { get; init; } = 0;
    public decimal Value { get; init; } = 0;
    public Dictionary<string, object>? SpecializedProperties { get; init; } // Pour les propriétés D&D
}