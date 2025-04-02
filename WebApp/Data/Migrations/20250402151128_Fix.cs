using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCasesJira_TestCases_TestCasesId",
                table: "TestCasesJira");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCasesJira_TestCases_TestCasesJiraId",
                table: "TestCasesJira");

            migrationBuilder.DropIndex(
                name: "IX_TestCasesJira_TestCasesJiraId",
                table: "TestCasesJira");

            migrationBuilder.DropColumn(
                name: "TestCasesJiraId",
                table: "TestCasesJira");

            migrationBuilder.AlterColumn<int>(
                name: "TestCasesId",
                table: "TestCasesJira",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TestCasesJira_TestCases_TestCasesId",
                table: "TestCasesJira",
                column: "TestCasesId",
                principalTable: "TestCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCasesJira_TestCases_TestCasesId",
                table: "TestCasesJira");

            migrationBuilder.AlterColumn<int>(
                name: "TestCasesId",
                table: "TestCasesJira",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "TestCasesJiraId",
                table: "TestCasesJira",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestCasesJira_TestCasesJiraId",
                table: "TestCasesJira",
                column: "TestCasesJiraId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCasesJira_TestCases_TestCasesId",
                table: "TestCasesJira",
                column: "TestCasesId",
                principalTable: "TestCases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCasesJira_TestCases_TestCasesJiraId",
                table: "TestCasesJira",
                column: "TestCasesJiraId",
                principalTable: "TestCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
