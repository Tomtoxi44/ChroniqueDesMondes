using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cdm.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddCompleteSystemsWithRestrictedForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentBlocks_ACharacter_CharacterId",
                table: "ContentBlocks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ContentBlocks",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "CampaignInvitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InvitedById = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignInvitations_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignInvitations_Users_InvitedById",
                        column: x => x.InvitedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignInvitations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CharacterSpells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    SpellId = table.Column<int>(type: "int", nullable: false),
                    LearnedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsPrepared = table.Column<bool>(type: "bit", nullable: false),
                    SpellSlotLevel = table.Column<int>(type: "int", nullable: true),
                    CustomName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSpells", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CombatSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChapterId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GameType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CurrentRound = table.Column<int>(type: "int", nullable: false),
                    CurrentTurnIndex = table.Column<int>(type: "int", nullable: false),
                    TurnOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Settings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Environment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionLog = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TurnTimeLimit = table.Column<TimeSpan>(type: "time", nullable: true),
                    CurrentTurnStarted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatSessions_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GameType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Tags = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    WeaponType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Damage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DamageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AttackBonus = table.Column<int>(type: "int", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ArmorClass = table.Column<int>(type: "int", nullable: true),
                    MaxDexBonus = table.Column<int>(type: "int", nullable: true, defaultValue: -1),
                    StealthDisadvantage = table.Column<bool>(type: "bit", nullable: true),
                    StrengthRequirement = table.Column<int>(type: "int", nullable: true),
                    IsWeapon = table.Column<bool>(type: "bit", nullable: true),
                    IsArmor = table.Column<bool>(type: "bit", nullable: true),
                    IsShield = table.Column<bool>(type: "bit", nullable: true),
                    IsMagical = table.Column<bool>(type: "bit", nullable: true),
                    RequiresAttunement = table.Column<bool>(type: "bit", nullable: true),
                    Rarity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Common"),
                    MagicalProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Charges = table.Column<int>(type: "int", nullable: true),
                    IsConsumable = table.Column<bool>(type: "bit", nullable: true),
                    Effect = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipment_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Spells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GameType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Tags = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    School = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: true),
                    CastingTime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Range = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Components = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AttackRoll = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Damage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SavingThrow = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsRitual = table.Column<bool>(type: "bit", nullable: true),
                    RequiresConcentration = table.Column<bool>(type: "bit", nullable: true),
                    MaterialComponent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HigherLevelDamage = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spells_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Permissions = table.Column<int>(type: "int", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    InvitationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignParticipants_CampaignInvitations_InvitationId",
                        column: x => x.InvitationId,
                        principalTable: "CampaignInvitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CampaignParticipants_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CampaignParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CombatParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CombatId = table.Column<int>(type: "int", nullable: false),
                    ParticipantType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    NpcId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Initiative = table.Column<int>(type: "int", nullable: true),
                    InitiativeTiebreaker = table.Column<int>(type: "int", nullable: false),
                    CurrentHitPoints = table.Column<int>(type: "int", nullable: false),
                    MaxHitPoints = table.Column<int>(type: "int", nullable: false),
                    TemporaryHitPoints = table.Column<int>(type: "int", nullable: false),
                    ArmorClass = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsConscious = table.Column<bool>(type: "bit", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StatusEffects = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resources = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvailableActions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameSpecificData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTurnAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TurnsPlayed = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatParticipants_CombatSessions_CombatId",
                        column: x => x.CombatId,
                        principalTable: "CombatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterInventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsEquipped = table.Column<bool>(type: "bit", nullable: false),
                    ObtainedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterInventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterInventory_ACharacter_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "ACharacter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterInventory_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentOffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    GameMasterId = table.Column<int>(type: "int", nullable: false),
                    TargetPlayerId = table.Column<int>(type: "int", nullable: false),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentOffers_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentOffers_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentOffers_Users_GameMasterId",
                        column: x => x.GameMasterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentOffers_Users_TargetPlayerId",
                        column: x => x.TargetPlayerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentTrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    FromPlayerId = table.Column<int>(type: "int", nullable: false),
                    ToPlayerId = table.Column<int>(type: "int", nullable: false),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentTrades_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentTrades_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentTrades_Users_FromPlayerId",
                        column: x => x.FromPlayerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentTrades_Users_ToPlayerId",
                        column: x => x.ToPlayerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CombatActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CombatId = table.Column<int>(type: "int", nullable: false),
                    ActorId = table.Column<int>(type: "int", nullable: false),
                    TargetId = table.Column<int>(type: "int", nullable: true),
                    Round = table.Column<int>(type: "int", nullable: false),
                    ActionSequence = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EquipmentId = table.Column<int>(type: "int", nullable: true),
                    SpellId = table.Column<int>(type: "int", nullable: true),
                    SpellLevel = table.Column<int>(type: "int", nullable: true),
                    DiceRolls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DamageDealt = table.Column<int>(type: "int", nullable: true),
                    DamageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HealingDone = table.Column<int>(type: "int", nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: true),
                    IsCritical = table.Column<bool>(type: "bit", nullable: false),
                    AdvantageType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SavingThrow = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Effects = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourcesUsed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionBefore = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PositionAfter = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MovementDistance = table.Column<int>(type: "int", nullable: true),
                    GameSpecificData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionTime = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatActions_CombatParticipants_ActorId",
                        column: x => x.ActorId,
                        principalTable: "CombatParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CombatActions_CombatParticipants_TargetId",
                        column: x => x.TargetId,
                        principalTable: "CombatParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CombatActions_CombatSessions_CombatId",
                        column: x => x.CombatId,
                        principalTable: "CombatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CombatStatusEffects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EffectType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SourceParticipantId = table.Column<int>(type: "int", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    InitialDuration = table.Column<int>(type: "int", nullable: false),
                    Intensity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Modifiers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PeriodicEffect = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SavingThrow = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CanBeDispelled = table.Column<bool>(type: "bit", nullable: false),
                    IsStackable = table.Column<bool>(type: "bit", nullable: false),
                    AppliedAtRound = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatStatusEffects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatStatusEffects_CombatParticipants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "CombatParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CombatStatusEffects_CombatParticipants_SourceParticipantId",
                        column: x => x.SourceParticipantId,
                        principalTable: "CombatParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentBlocks_ChapterId_Order",
                table: "ContentBlocks",
                columns: new[] { "ChapterId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentBlocks_Type",
                table: "ContentBlocks",
                column: "Type");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ContentBlocks_NpcDialogue",
                table: "ContentBlocks",
                sql: "([Type] = 'NpcDialogue' AND [CharacterId] IS NOT NULL AND [NpcMood] IS NOT NULL) OR ([Type] != 'NpcDialogue' AND [CharacterId] IS NULL AND [NpcMood] IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ContentBlocks_Type",
                table: "ContentBlocks",
                sql: "[Type] IN ('Location', 'NpcDialogue', 'Description', 'Event')");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignInvitations_CampaignId_Email",
                table: "CampaignInvitations",
                columns: new[] { "CampaignId", "Email" },
                unique: true,
                filter: "[Status] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignInvitations_InvitedById",
                table: "CampaignInvitations",
                column: "InvitedById");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignInvitations_Token",
                table: "CampaignInvitations",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignInvitations_UserId",
                table: "CampaignInvitations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignParticipants_CampaignId_UserId",
                table: "CampaignParticipants",
                columns: new[] { "CampaignId", "UserId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignParticipants_InvitationId",
                table: "CampaignParticipants",
                column: "InvitationId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignParticipants_UserId",
                table: "CampaignParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInventory_Character_Equipment",
                table: "CharacterInventory",
                columns: new[] { "CharacterId", "EquipmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInventory_Character_Equipped",
                table: "CharacterInventory",
                columns: new[] { "CharacterId", "IsEquipped" });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInventory_CharacterId",
                table: "CharacterInventory",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInventory_EquipmentId",
                table: "CharacterInventory",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSpells_Character_Prepared",
                table: "CharacterSpells",
                columns: new[] { "CharacterId", "IsPrepared" });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSpells_Character_Spell",
                table: "CharacterSpells",
                columns: new[] { "CharacterId", "SpellId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSpells_CharacterId",
                table: "CharacterSpells",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSpells_SpellId",
                table: "CharacterSpells",
                column: "SpellId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatActions_ActorId",
                table: "CombatActions",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatActions_CombatId",
                table: "CombatActions",
                column: "CombatId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatActions_CombatId_Round",
                table: "CombatActions",
                columns: new[] { "CombatId", "Round" });

            migrationBuilder.CreateIndex(
                name: "IX_CombatActions_ExecutedAt",
                table: "CombatActions",
                column: "ExecutedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CombatActions_TargetId",
                table: "CombatActions",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatParticipants_CombatId",
                table: "CombatParticipants",
                column: "CombatId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatParticipants_CombatId_Initiative",
                table: "CombatParticipants",
                columns: new[] { "CombatId", "Initiative" });

            migrationBuilder.CreateIndex(
                name: "IX_CombatParticipants_CombatId_ParticipantType",
                table: "CombatParticipants",
                columns: new[] { "CombatId", "ParticipantType" });

            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_ChapterId",
                table: "CombatSessions",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_SessionId",
                table: "CombatSessions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_SessionId_Status",
                table: "CombatSessions",
                columns: new[] { "SessionId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_Status",
                table: "CombatSessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CombatStatusEffects_EffectType",
                table: "CombatStatusEffects",
                column: "EffectType");

            migrationBuilder.CreateIndex(
                name: "IX_CombatStatusEffects_ParticipantId",
                table: "CombatStatusEffects",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatStatusEffects_ParticipantId_IsActive",
                table: "CombatStatusEffects",
                columns: new[] { "ParticipantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CombatStatusEffects_SourceParticipantId",
                table: "CombatStatusEffects",
                column: "SourceParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_ArmorClass",
                table: "Equipment",
                column: "ArmorClass");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_AttackBonus",
                table: "Equipment",
                column: "AttackBonus");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Category",
                table: "Equipment",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Category_GameType",
                table: "Equipment",
                columns: new[] { "Category", "GameType" });

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_CreatedByUserId",
                table: "Equipment",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_DamageType",
                table: "Equipment",
                column: "DamageType");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_GameType",
                table: "Equipment",
                column: "GameType");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_GameType_IsPublic_IsActive",
                table: "Equipment",
                columns: new[] { "GameType", "IsPublic", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Name",
                table: "Equipment",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Rarity",
                table: "Equipment",
                column: "Rarity");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Type_Flags",
                table: "Equipment",
                columns: new[] { "IsWeapon", "IsArmor" });

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_WeaponType",
                table: "Equipment",
                column: "WeaponType");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentOffers_Campaign_Player_Status",
                table: "EquipmentOffers",
                columns: new[] { "CampaignId", "TargetPlayerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentOffers_CampaignId",
                table: "EquipmentOffers",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentOffers_EquipmentId",
                table: "EquipmentOffers",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentOffers_GameMasterId",
                table: "EquipmentOffers",
                column: "GameMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentOffers_Status",
                table: "EquipmentOffers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentOffers_TargetPlayerId",
                table: "EquipmentOffers",
                column: "TargetPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTrades_Campaign_Status",
                table: "EquipmentTrades",
                columns: new[] { "CampaignId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTrades_CampaignId",
                table: "EquipmentTrades",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTrades_EquipmentId",
                table: "EquipmentTrades",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTrades_FromPlayerId",
                table: "EquipmentTrades",
                column: "FromPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTrades_Status",
                table: "EquipmentTrades",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentTrades_ToPlayerId",
                table: "EquipmentTrades",
                column: "ToPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_CreatedByUserId",
                table: "Spells",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_GameType",
                table: "Spells",
                column: "GameType");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_GameType_IsPublic_IsActive",
                table: "Spells",
                columns: new[] { "GameType", "IsPublic", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Spells_Level",
                table: "Spells",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_Level_School",
                table: "Spells",
                columns: new[] { "Level", "School" });

            migrationBuilder.CreateIndex(
                name: "IX_Spells_Name",
                table: "Spells",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_School",
                table: "Spells",
                column: "School");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentBlocks_ACharacter_CharacterId",
                table: "ContentBlocks",
                column: "CharacterId",
                principalTable: "ACharacter",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentBlocks_ACharacter_CharacterId",
                table: "ContentBlocks");

            migrationBuilder.DropTable(
                name: "CampaignParticipants");

            migrationBuilder.DropTable(
                name: "CharacterInventory");

            migrationBuilder.DropTable(
                name: "CharacterSpells");

            migrationBuilder.DropTable(
                name: "CombatActions");

            migrationBuilder.DropTable(
                name: "CombatStatusEffects");

            migrationBuilder.DropTable(
                name: "EquipmentOffers");

            migrationBuilder.DropTable(
                name: "EquipmentTrades");

            migrationBuilder.DropTable(
                name: "Spells");

            migrationBuilder.DropTable(
                name: "CampaignInvitations");

            migrationBuilder.DropTable(
                name: "CombatParticipants");

            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "CombatSessions");

            migrationBuilder.DropIndex(
                name: "IX_ContentBlocks_ChapterId_Order",
                table: "ContentBlocks");

            migrationBuilder.DropIndex(
                name: "IX_ContentBlocks_Type",
                table: "ContentBlocks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ContentBlocks_NpcDialogue",
                table: "ContentBlocks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ContentBlocks_Type",
                table: "ContentBlocks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ContentBlocks",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentBlocks_ACharacter_CharacterId",
                table: "ContentBlocks",
                column: "CharacterId",
                principalTable: "ACharacter",
                principalColumn: "Id");
        }
    }
}
