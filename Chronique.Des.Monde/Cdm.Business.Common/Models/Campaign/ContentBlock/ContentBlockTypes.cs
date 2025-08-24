namespace Cdm.Business.Common.Models.Campaign.ContentBlock;

// Types de blocs disponibles
public static class ContentBlockTypes
{
    public const string Location = "Location";
    public const string NpcDialogue = "NpcDialogue";
    public const string Description = "Description";
    public const string Event = "Event";
    
    public static readonly List<string> All = new()
    {
        Location, NpcDialogue, Description, Event
    };
}

// Humeurs disponibles pour les NPCs
public static class NpcMoods
{
    public const string Hostile = "Hostile";
    public const string Neutral = "Neutral";
    public const string Friendly = "Friendly";
    public const string Scared = "Scared";
    public const string Angry = "Angry";
    public const string Suspicious = "Suspicious";
    public const string Helpful = "Helpful";
    
    public static readonly List<string> All = new()
    {
        Hostile, Neutral, Friendly, Scared, Angry, Suspicious, Helpful
    };
}