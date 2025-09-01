using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cdm.Data.Dnd.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentExchangeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipmentOffers");

            migrationBuilder.DropTable(
                name: "EquipmentTrades");
        }
    }
}
