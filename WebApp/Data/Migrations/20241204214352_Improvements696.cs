using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class Improvements696 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequirementsFiles_Requirements_RequirementsId1",
                table: "RequirementsFiles");

            migrationBuilder.DropIndex(
                name: "IX_RequirementsFiles_RequirementsId1",
                table: "RequirementsFiles");

            migrationBuilder.DropColumn(
                name: "RequirementsId1",
                table: "RequirementsFiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequirementsId1",
                table: "RequirementsFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequirementsFiles_RequirementsId1",
                table: "RequirementsFiles",
                column: "RequirementsId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementsFiles_Requirements_RequirementsId1",
                table: "RequirementsFiles",
                column: "RequirementsId1",
                principalTable: "Requirements",
                principalColumn: "Id");
        }
    }
}
