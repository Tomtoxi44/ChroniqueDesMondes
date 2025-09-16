using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cdm.Data.Models;

namespace Cdm.Data.Common.Models;

// Modèles pour les statistiques et succès
[Table("PlayerStats")]
public class PlayerStat
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    [Required, MaxLength(50)]
    public string StatType { get; set; } = string.Empty;
    
    public decimal StatValue { get; set; }
    public int? GameType { get; set; }
    public int? CharacterId { get; set; }
    public int? CampaignId { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public string? Metadata { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    
    [ForeignKey(nameof(CharacterId))]
    public virtual ACharacter? Character { get; set; }
    
    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign? Campaign { get; set; }
}

[Table("Achievements")]
public class Achievement
{
    [Key, MaxLength(100)]
    public string Id { get; set; } = string.Empty;
    
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required, MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required, MaxLength(10)]
    public string Icon { get; set; } = string.Empty;
    
    [Required, MaxLength(50)]
    public string Category { get; set; } = string.Empty;
    
    [Required, MaxLength(20)]
    public string Rarity { get; set; } = string.Empty;
    
    public int? GameType { get; set; }
    
    [Required, MaxLength(50)]
    public string RequirementType { get; set; } = string.Empty;
    
    public decimal RequirementValue { get; set; }
    public string? RequirementMetadata { get; set; }
    public int ExperienceReward { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>();
}

[Table("PlayerAchievements")]
public class PlayerAchievement
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    [Required, MaxLength(100)]
    public string AchievementId { get; set; } = string.Empty;
    
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    public int? CharacterId { get; set; }
    public int? CampaignId { get; set; }
    public int? CombatSessionId { get; set; }
    public string? ContextData { get; set; }
    
    [MaxLength(500)]
    public string? Witnesses { get; set; }
    
    public bool IsShared { get; set; } = false;

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    
    [ForeignKey(nameof(AchievementId))]
    public virtual Achievement Achievement { get; set; } = null!;
    
    [ForeignKey(nameof(CharacterId))]
    public virtual ACharacter? Character { get; set; }
    
    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign? Campaign { get; set; }
}

[Table("DiceRolls")]
public class DiceRoll
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? CharacterId { get; set; }
    public int? CampaignId { get; set; }
    public int? CombatSessionId { get; set; }
    public int DiceType { get; set; }
    public int RollResult { get; set; }
    
    [Required, MaxLength(50)]
    public string RollPurpose { get; set; } = string.Empty;
    
    public int Modifier { get; set; } = 0;
    public int TotalResult { get; set; }
    public bool IsAdvantage { get; set; } = false;
    public bool IsDisadvantage { get; set; } = false;
    public bool IsCritical { get; set; } = false;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [MaxLength(200)]
    public string? Context { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    
    [ForeignKey(nameof(CharacterId))]
    public virtual ACharacter? Character { get; set; }
    
    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign? Campaign { get; set; }
}

// Modèles pour les sessions de jeu
[Table("GameSessions")]
public class GameSession
{
    [Key, MaxLength(100)]
    public string Id { get; set; } = string.Empty;
    
    public int CampaignId { get; set; }
    public int GameMasterId { get; set; }
    
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required, MaxLength(20)]
    public string Status { get; set; } = "Preparing";
    
    public int? CurrentChapterId { get; set; }
    public int MaxPlayers { get; set; } = 6;
    public bool IsPublic { get; set; } = false;
    
    [MaxLength(20)]
    public string? InviteCode { get; set; }
    
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public string? Settings { get; set; }

    // Navigation properties
    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; } = null!;
    
    [ForeignKey(nameof(GameMasterId))]
    public virtual User GameMaster { get; set; } = null!;
    
    [ForeignKey(nameof(CurrentChapterId))]
    public virtual Chapter? CurrentChapter { get; set; }
    
    public virtual ICollection<SessionParticipant> Participants { get; set; } = new List<SessionParticipant>();
    public virtual ICollection<SessionState> States { get; set; } = new List<SessionState>();
    public virtual ICollection<SessionInvitation> Invitations { get; set; } = new List<SessionInvitation>();
}

[Table("SessionParticipants")]
public class SessionParticipant
{
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string SessionId { get; set; } = string.Empty;
    
    public int UserId { get; set; }
    public int? CharacterId { get; set; }
    
    [Required, MaxLength(20)]
    public string Role { get; set; } = "Player";
    
    [Required, MaxLength(20)]
    public string Status { get; set; } = "Invited";
    
    public DateTime? JoinedAt { get; set; }
    public DateTime? LastSeenAt { get; set; }
    public bool IsOnline { get; set; } = false;
    
    [MaxLength(100)]
    public string? ConnectionId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(SessionId))]
    public virtual GameSession Session { get; set; } = null!;
    
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    
    [ForeignKey(nameof(CharacterId))]
    public virtual ACharacter? Character { get; set; }
}

[Table("SessionStates")]
public class SessionState
{
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string SessionId { get; set; } = string.Empty;
    
    public int SlotNumber { get; set; }
    
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public string StateData { get; set; } = string.Empty;
    
    public bool IsAutoSave { get; set; } = false;
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long FileSize { get; set; } = 0;

    // Navigation properties
    [ForeignKey(nameof(SessionId))]
    public virtual GameSession Session { get; set; } = null!;
    
    [ForeignKey(nameof(CreatedBy))]
    public virtual User Creator { get; set; } = null!;
}

[Table("SessionInvitations")]
public class SessionInvitation
{
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string SessionId { get; set; } = string.Empty;
    
    public int InvitedUserId { get; set; }
    public int InvitedBy { get; set; }
    
    [Required, MaxLength(20)]
    public string InviteType { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Message { get; set; }
    
    [Required, MaxLength(20)]
    public string Status { get; set; } = "Pending";
    
    public DateTime? ExpiresAt { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(SessionId))]
    public virtual GameSession Session { get; set; } = null!;
    
    [ForeignKey(nameof(InvitedUserId))]
    public virtual User InvitedUser { get; set; } = null!;
    
    [ForeignKey(nameof(InvitedBy))]
    public virtual User Inviter { get; set; } = null!;
}