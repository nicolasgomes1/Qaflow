using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestExecutionId1",
                table: "TestCaseExecution",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestCaseExecution_TestExecutionId1",
                table: "TestCaseExecution",
                column: "TestExecutionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCaseExecution_TestExecution_TestExecutionId1",
                table: "TestCaseExecution",
                column: "TestExecutionId1",
                principalTable: "TestExecution",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCaseExecution_TestExecution_TestExecutionId1",
                table: "TestCaseExecution");

            migrationBuilder.DropIndex(
                name: "IX_TestCaseExecution_TestExecutionId1",
                table: "TestCaseExecution");

            migrationBuilder.DropColumn(
                name: "TestExecutionId1",
                table: "TestCaseExecution");
        }
    }
}
