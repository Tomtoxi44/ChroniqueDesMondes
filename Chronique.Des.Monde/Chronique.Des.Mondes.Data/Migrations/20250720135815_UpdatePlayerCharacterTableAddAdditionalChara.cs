using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronique.Des.Mondes.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlayerCharacterTableAddAdditionalChara : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdditionalCharism",
                table: "PlayerCharacter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalConstitution",
                table: "PlayerCharacter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalDexterity",
                table: "PlayerCharacter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalIntelligence",
                table: "PlayerCharacter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalStrong",
                table: "PlayerCharacter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdditionalWisdoms",
                table: "PlayerCharacter",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalCharism",
                table: "PlayerCharacter");

            migrationBuilder.DropColumn(
                name: "AdditionalConstitution",
                table: "PlayerCharacter");

            migrationBuilder.DropColumn(
                name: "AdditionalDexterity",
                table: "PlayerCharacter");

            migrationBuilder.DropColumn(
                name: "AdditionalIntelligence",
                table: "PlayerCharacter");

            migrationBuilder.DropColumn(
                name: "AdditionalStrong",
                table: "PlayerCharacter");

            migrationBuilder.DropColumn(
                name: "AdditionalWisdoms",
                table: "PlayerCharacter");
        }
    }
}
