using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cdm.Data.Dnd.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentDndSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Créer uniquement la table Equipment qui n'existe pas encore
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

            // Index pour les performances
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipment");
        }
    }
}
