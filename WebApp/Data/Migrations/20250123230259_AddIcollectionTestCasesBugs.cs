using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIcollectionTestCasesBugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BugsId",
                table: "TestCases",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestCases_BugsId",
                table: "TestCases",
                column: "BugsId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCases_Bugs_BugsId",
                table: "TestCases",
                column: "BugsId",
                principalTable: "Bugs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_Bugs_BugsId",
                table: "TestCases");

            migrationBuilder.DropIndex(
                name: "IX_TestCases_BugsId",
                table: "TestCases");

            migrationBuilder.DropColumn(
                name: "BugsId",
                table: "TestCases");
        }
    }
}
