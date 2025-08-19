namespace Cdm.Web.Models;

public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string CharacterClass { get; set; } = "";
    public string Race { get; set; } = "";
    public int Level { get; set; } = 1;
    public int HitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int ArmorClass { get; set; }
    public int UserId { get; set; }
    public string GameType { get; set; } = "Dnd";
}

public class CharacterRequest
{
    public string Name { get; set; } = "";
    public string CharacterClass { get; set; } = "";
    public string Race { get; set; } = "";
    public int Level { get; set; } = 1;
    public string GameType { get; set; } = "Dnd";
}

public class CharacterDndRequest : CharacterRequest
{
    public int Strength { get; set; } = 10;
    public int Dexterity { get; set; } = 10;
    public int Constitution { get; set; } = 10;
    public int Intelligence { get; set; } = 10;
    public int Wisdom { get; set; } = 10;
    public int Charisma { get; set; } = 10;
}