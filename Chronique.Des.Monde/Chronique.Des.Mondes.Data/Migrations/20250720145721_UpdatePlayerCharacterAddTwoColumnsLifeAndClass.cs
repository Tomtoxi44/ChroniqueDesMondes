using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronique.Des.Mondes.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlayerCharacterAddTwoColumnsLifeAndClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "PlayerCharacter",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Life",
                table: "PlayerCharacter",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Class",
                table: "PlayerCharacter");

            migrationBuilder.DropColumn(
                name: "Life",
                table: "PlayerCharacter");
        }
    }
}
