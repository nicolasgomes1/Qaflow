using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class dsffffff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestStepsExecution_TestCaseExecution_TestCaseExecutionId",
                table: "TestStepsExecution");

            migrationBuilder.AddForeignKey(
                name: "FK_TestStepsExecution_TestCaseExecution_TestCaseExecutionId",
                table: "TestStepsExecution",
                column: "TestCaseExecutionId",
                principalTable: "TestCaseExecution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestStepsExecution_TestCaseExecution_TestCaseExecutionId",
                table: "TestStepsExecution");

            migrationBuilder.AddForeignKey(
                name: "FK_TestStepsExecution_TestCaseExecution_TestCaseExecutionId",
                table: "TestStepsExecution",
                column: "TestCaseExecutionId",
                principalTable: "TestCaseExecution",
                principalColumn: "Id");
        }
    }
}
