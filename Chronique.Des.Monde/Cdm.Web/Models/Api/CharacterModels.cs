namespace Cdm.Web.Models.Api;

/// <summary>
/// Modèles de données pour les personnages D&D
/// </summary>
public record CharacterDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Race { get; init; } = "";
    public string Class { get; init; } = "";
    public int Level { get; init; }
    public int HitPoints { get; init; }
    public int MaxHitPoints { get; init; }
    public int ArmorClass { get; init; }
    public int Speed { get; init; }
    
    // Caractéristiques
    public int Strength { get; init; }
    public int Dexterity { get; init; }
    public int Constitution { get; init; }
    public int Intelligence { get; init; }
    public int Wisdom { get; init; }
    public int Charisma { get; init; }
    
    // Bonus/malus additionnels
    public int AdditionalStrength { get; init; }
    public int AdditionalDexterity { get; init; }
    public int AdditionalConstitution { get; init; }
    public int AdditionalIntelligence { get; init; }
    public int AdditionalWisdoms { get; init; }
    public int AdditionalCharism { get; init; }
    
    public int UserId { get; init; }
    public string Picture { get; init; } = "";
    public string? Background { get; init; }
    public int Life { get; init; }
    public int Leveling { get; init; }
    public int ClassArmor { get; init; }
}

/// <summary>
/// Requête pour créer un nouveau personnage
/// </summary>
public record CreateCharacterRequest
{
    public string Name { get; init; } = "";
    public string Race { get; init; } = "";
    public string Class { get; init; } = "";
    public int Level { get; init; } = 1;
    public int MaxHitPoints { get; init; } = 10;
    public int ArmorClass { get; init; } = 10;
    public int Speed { get; init; } = 30;
    
    // Caractéristiques de base (entre 8 et 15 généralement)
    public int Strength { get; init; } = 10;
    public int Dexterity { get; init; } = 10;
    public int Constitution { get; init; } = 10;
    public int Intelligence { get; init; } = 10;
    public int Wisdom { get; init; } = 10;
    public int Charisma { get; init; } = 10;
    
    public string Picture { get; init; } = "";
    public string? Background { get; init; }
}

/// <summary>
/// Requête pour mettre à jour un personnage
/// </summary>
public record UpdateCharacterRequest
{
    public string? Name { get; init; }
    public int? HitPoints { get; init; }
    public int? MaxHitPoints { get; init; }
    public int? Level { get; init; }
    public string? Picture { get; init; }
    public string? Background { get; init; }
    
    // Bonus d'équipement ou temporaires
    public int? AdditionalStrength { get; init; }
    public int? AdditionalDexterity { get; init; }
    public int? AdditionalConstitution { get; init; }
    public int? AdditionalIntelligence { get; init; }
    public int? AdditionalWisdoms { get; init; }
    public int? AdditionalCharism { get; init; }
}

/// <summary>
/// Réponse d'authentification
/// </summary>
public record AuthResponse
{
    public bool Success { get; init; }
    public string? Token { get; init; }
    public UserDto? User { get; init; }
    public string? Error { get; init; }
}

/// <summary>
/// Modèle utilisateur
/// </summary>
public record UserDto
{
    public int Id { get; init; }
    public string UserName { get; init; } = "";
    public string UserEmail { get; init; } = "";
}

/// <summary>
/// Requête de connexion
/// </summary>
public record LoginRequest
{
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
}

/// <summary>
/// Requête d'inscription
/// </summary>
public record RegisterRequest
{
    public string UserName { get; init; } = "";
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
}