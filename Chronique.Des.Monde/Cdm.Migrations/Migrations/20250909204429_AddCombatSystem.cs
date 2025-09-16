using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cdm.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddCombatSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Table pour les sessions de combat
            migrationBuilder.CreateTable(
                name: "CombatSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChapterId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GameType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "initiative"),
                    CurrentRound = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CurrentTurnIndex = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TurnOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Settings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Environment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionLog = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
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

            // Table pour les participants au combat
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
                    InitiativeTiebreaker = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CurrentHitPoints = table.Column<int>(type: "int", nullable: false),
                    MaxHitPoints = table.Column<int>(type: "int", nullable: false),
                    TemporaryHitPoints = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ArmorClass = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<int>(type: "int", nullable: false, defaultValue: 9),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsConscious = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StatusEffects = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resources = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvailableActions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameSpecificData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTurnAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TurnsPlayed = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatParticipants_CombatSessions_CombatId",
                        column: x => x.CombatId,
                        principalTable: "CombatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Table pour les actions de combat
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
                    ActionSequence = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
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
                    IsCritical = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AdvantageType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SavingThrow = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Effects = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourcesUsed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionBefore = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PositionAfter = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MovementDistance = table.Column<int>(type: "int", nullable: true),
                    GameSpecificData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExecutionTime = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatActions_CombatSessions_CombatId",
                        column: x => x.CombatId,
                        principalTable: "CombatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                });

            // Index pour les performances
            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_ChapterId",
                table: "CombatSessions",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_Status",
                table: "CombatSessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_SessionId",
                table: "CombatSessions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatParticipants_CombatId",
                table: "CombatParticipants",
                column: "CombatId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatParticipants_Initiative",
                table: "CombatParticipants",
                column: "Initiative");

            migrationBuilder.CreateIndex(
                name: "IX_CombatParticipants_ParticipantType",
                table: "CombatParticipants",
                column: "ParticipantType");

            migrationBuilder.CreateIndex(
                name: "IX_CombatParticipants_IsActive",
                table: "CombatParticipants",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CombatActions_CombatId_Round",
                table: "CombatActions",
                columns: new[] { "CombatId", "Round" });

            migrationBuilder.CreateIndex(
                name: "IX_CombatActions_ActorId",
                table: "CombatActions",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatActions_TargetId",
                table: "CombatActions",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatActions_ActionType",
                table: "CombatActions",
                column: "ActionType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CombatActions");

            migrationBuilder.DropTable(
                name: "CombatParticipants");

            migrationBuilder.DropTable(
                name: "CombatSessions");
        }
    }
}
