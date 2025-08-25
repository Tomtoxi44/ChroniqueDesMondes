using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cdm.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddContentBlocksAndUnifyCharacterNpc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ajouter les nouvelles colonnes aux Characters pour supporter les NPCs
            migrationBuilder.AddColumn<bool>(
                name: "IsNpc",
                table: "CharactersDnd",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ChapterId",
                table: "CharactersDnd",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "CharactersDnd",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHostile",
                table: "CharactersDnd",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemCharacter",
                table: "CharactersDnd",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "GameType",
                table: "CharactersDnd",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CharactersDnd",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            // Mettre à jour le GameType dans Campaigns pour utiliser l'enum
            migrationBuilder.AlterColumn<int>(
                name: "GameType",
                table: "Campaigns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            // Créer la table ContentBlocks
            migrationBuilder.CreateTable(
                name: "ContentBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChapterId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CharacterId = table.Column<int>(type: "int", nullable: true),
                    NpcMood = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentBlocks_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentBlocks_CharactersDnd_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "CharactersDnd",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.CheckConstraint("CK_ContentBlocks_Type", "[Type] IN ('Location', 'NpcDialogue', 'Description', 'Event')");
                    table.CheckConstraint("CK_ContentBlocks_NpcDialogue", "([Type] = 'NpcDialogue' AND [CharacterId] IS NOT NULL AND [NpcMood] IS NOT NULL) OR ([Type] != 'NpcDialogue' AND [CharacterId] IS NULL AND [NpcMood] IS NULL)");
                });

            // Ajouter la foreign key pour Chapter vers Character (NPCs)
            migrationBuilder.CreateIndex(
                name: "IX_CharactersDnd_ChapterId",
                table: "CharactersDnd",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharactersDnd_IsNpc",
                table: "CharactersDnd",
                column: "IsNpc");

            migrationBuilder.CreateIndex(
                name: "IX_CharactersDnd_IsHostile",
                table: "CharactersDnd",
                column: "IsHostile");

            migrationBuilder.CreateIndex(
                name: "IX_CharactersDnd_GameType",
                table: "CharactersDnd",
                column: "GameType");

            migrationBuilder.CreateIndex(
                name: "IX_CharactersDnd_IsSystemCharacter",
                table: "CharactersDnd",
                column: "IsSystemCharacter");

            // ContentBlocks indexes
            migrationBuilder.CreateIndex(
                name: "IX_ContentBlocks_ChapterId",
                table: "ContentBlocks",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentBlocks_Type",
                table: "ContentBlocks",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ContentBlocks_CharacterId",
                table: "ContentBlocks",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentBlocks_ChapterId_Order",
                table: "ContentBlocks",
                columns: new[] { "ChapterId", "Order" },
                unique: true);

            // Ajouter la foreign key pour les NPCs vers les chapitres
            migrationBuilder.AddForeignKey(
                name: "FK_CharactersDnd_Chapters_ChapterId",
                table: "CharactersDnd",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Supprimer la table ContentBlocks
            migrationBuilder.DropTable(
                name: "ContentBlocks");

            // Supprimer les foreign keys
            migrationBuilder.DropForeignKey(
                name: "FK_CharactersDnd_Chapters_ChapterId",
                table: "CharactersDnd");

            // Supprimer les indexes
            migrationBuilder.DropIndex(
                name: "IX_CharactersDnd_ChapterId",
                table: "CharactersDnd");

            migrationBuilder.DropIndex(
                name: "IX_CharactersDnd_IsNpc",
                table: "CharactersDnd");

            migrationBuilder.DropIndex(
                name: "IX_CharactersDnd_IsHostile",
                table: "CharactersDnd");

            migrationBuilder.DropIndex(
                name: "IX_CharactersDnd_GameType",
                table: "CharactersDnd");

            migrationBuilder.DropIndex(
                name: "IX_CharactersDnd_IsSystemCharacter",
                table: "CharactersDnd");

            // Supprimer les nouvelles colonnes des Characters
            migrationBuilder.DropColumn(
                name: "IsNpc",
                table: "CharactersDnd");

            migrationBuilder.DropColumn(
                name: "ChapterId",
                table: "CharactersDnd");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "CharactersDnd");

            migrationBuilder.DropColumn(
                name: "IsHostile",
                table: "CharactersDnd");

            migrationBuilder.DropColumn(
                name: "IsSystemCharacter",
                table: "CharactersDnd");

            migrationBuilder.DropColumn(
                name: "GameType",
                table: "CharactersDnd");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CharactersDnd");
        }
    }
}