using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Simplify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCasesTestPlans_TestCases_TestCasesId",
                table: "TestCasesTestPlans");

            migrationBuilder.RenameColumn(
                name: "TestCasesId",
                table: "TestCasesTestPlans",
                newName: "LinkedTestCasesId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCasesTestPlans_TestCases_LinkedTestCasesId",
                table: "TestCasesTestPlans",
                column: "LinkedTestCasesId",
                principalTable: "TestCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCasesTestPlans_TestCases_LinkedTestCasesId",
                table: "TestCasesTestPlans");

            migrationBuilder.RenameColumn(
                name: "LinkedTestCasesId",
                table: "TestCasesTestPlans",
                newName: "TestCasesId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCasesTestPlans_TestCases_TestCasesId",
                table: "TestCasesTestPlans",
                column: "TestCasesId",
                principalTable: "TestCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
