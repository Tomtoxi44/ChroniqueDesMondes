namespace Cdm.Data.Models;

public enum ParticipantRole
{
    Player = 0,
    CoGameMaster = 1,
    Observer = 2
}

[Flags]
public enum ParticipantPermissions
{
    None = 0,
    ReadOnly = 1,
    CreateCharacters = 2,
    EditOwnCharacters = 4,
    ViewPrivateNotes = 8,
    EditCampaignContent = 16,
    ManageParticipants = 32,
    All = ReadOnly | CreateCharacters | EditOwnCharacters | ViewPrivateNotes | EditCampaignContent | ManageParticipants
}