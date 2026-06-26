using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DungeonQuest.Migrations
{
    /// <inheritdoc />
    public partial class RaritàArma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rarity",
                table: "Weapons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"ALTER TABLE `Weapons` MODIFY COLUMN `Rarity` int NOT NULL DEFAULT 0 AFTER `Damage`;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rarity",
                table: "Weapons");
        }
    }
}
