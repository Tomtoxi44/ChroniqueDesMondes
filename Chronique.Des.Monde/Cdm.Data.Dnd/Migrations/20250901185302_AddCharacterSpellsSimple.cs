using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cdm.Data.Dnd.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterSpellsSimple : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterInventory");

            migrationBuilder.DropTable(
                name: "CharacterSpells");
        }
    }
}
