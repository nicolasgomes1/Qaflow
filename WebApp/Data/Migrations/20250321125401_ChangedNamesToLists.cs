using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNamesToLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequirementsTestCases_Requirements_RequirementsId",
                table: "RequirementsTestCases");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementsTestCases_TestCases_TestCasesId",
                table: "RequirementsTestCases");

            migrationBuilder.RenameColumn(
                name: "TestCasesId",
                table: "RequirementsTestCases",
                newName: "LinkedTestCasesId");

            migrationBuilder.RenameColumn(
                name: "RequirementsId",
                table: "RequirementsTestCases",
                newName: "LinkedRequirementsId");

            migrationBuilder.RenameIndex(
                name: "IX_RequirementsTestCases_TestCasesId",
                table: "RequirementsTestCases",
                newName: "IX_RequirementsTestCases_LinkedTestCasesId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementsTestCases_Requirements_LinkedRequirementsId",
                table: "RequirementsTestCases",
                column: "LinkedRequirementsId",
                principalTable: "Requirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementsTestCases_TestCases_LinkedTestCasesId",
                table: "RequirementsTestCases",
                column: "LinkedTestCasesId",
                principalTable: "TestCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequirementsTestCases_Requirements_LinkedRequirementsId",
                table: "RequirementsTestCases");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementsTestCases_TestCases_LinkedTestCasesId",
                table: "RequirementsTestCases");

            migrationBuilder.RenameColumn(
                name: "LinkedTestCasesId",
                table: "RequirementsTestCases",
                newName: "TestCasesId");

            migrationBuilder.RenameColumn(
                name: "LinkedRequirementsId",
                table: "RequirementsTestCases",
                newName: "RequirementsId");

            migrationBuilder.RenameIndex(
                name: "IX_RequirementsTestCases_LinkedTestCasesId",
                table: "RequirementsTestCases",
                newName: "IX_RequirementsTestCases_TestCasesId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementsTestCases_Requirements_RequirementsId",
                table: "RequirementsTestCases",
                column: "RequirementsId",
                principalTable: "Requirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementsTestCases_TestCases_TestCasesId",
                table: "RequirementsTestCases",
                column: "TestCasesId",
                principalTable: "TestCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
