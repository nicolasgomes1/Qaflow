using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addTestCaseExecutionobject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Bugs_TestCaseExecutionId",
                table: "Bugs",
                column: "TestCaseExecutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bugs_TestCaseExecution_TestCaseExecutionId",
                table: "Bugs",
                column: "TestCaseExecutionId",
                principalTable: "TestCaseExecution",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bugs_TestCaseExecution_TestCaseExecutionId",
                table: "Bugs");

            migrationBuilder.DropIndex(
                name: "IX_Bugs_TestCaseExecutionId",
                table: "Bugs");
        }
    }
}
