using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DungeonQuest.Migrations
{
    /// <inheritdoc />
    public partial class TipiDa1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE `Weapons` SET `Type` = `Type` + 1 WHERE `Type` BETWEEN 0 AND 4;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE `Weapons` SET `Type` = `Type` - 1 WHERE `Type` BETWEEN 1 AND 5;");
        }
    }
}
