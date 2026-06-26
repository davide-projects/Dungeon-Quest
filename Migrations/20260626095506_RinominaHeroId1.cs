using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DungeonQuest.Migrations
{
    /// <inheritdoc />
    public partial class RinominaHeroId1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weapons_Heroes_HeroId1",
                table: "Weapons");

            migrationBuilder.RenameColumn(
                name: "HeroId1",
                table: "Weapons",
                newName: "EquippedByHeroId");

            migrationBuilder.RenameIndex(
                name: "IX_Weapons_HeroId1",
                table: "Weapons",
                newName: "IX_Weapons_EquippedByHeroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Weapons_Heroes_EquippedByHeroId",
                table: "Weapons",
                column: "EquippedByHeroId",
                principalTable: "Heroes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weapons_Heroes_EquippedByHeroId",
                table: "Weapons");

            migrationBuilder.RenameColumn(
                name: "EquippedByHeroId",
                table: "Weapons",
                newName: "HeroId1");

            migrationBuilder.RenameIndex(
                name: "IX_Weapons_EquippedByHeroId",
                table: "Weapons",
                newName: "IX_Weapons_HeroId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Weapons_Heroes_HeroId1",
                table: "Weapons",
                column: "HeroId1",
                principalTable: "Heroes",
                principalColumn: "Id");
        }
    }
}
