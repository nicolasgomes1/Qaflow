using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class FixedFkTestStepExecution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestStepsExecution_TestCaseExecution_TestCaseExecutionId",
                table: "TestStepsExecution");

            migrationBuilder.DropForeignKey(
                name: "FK_TestStepsExecution_TestCaseExecution_TestCaseExecutionId1",
                table: "TestStepsExecution");

            migrationBuilder.DropIndex(
                name: "IX_TestStepsExecution_TestCaseExecutionId1",
                table: "TestStepsExecution");

            migrationBuilder.DropColumn(
                name: "TestCaseExecutionId1",
                table: "TestStepsExecution");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseExecutionId",
                table: "TestStepsExecution",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TestCaseExecutionIdFk",
                table: "TestStepsExecution",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestStepsExecution_TestCaseExecutionIdFk",
                table: "TestStepsExecution",
                column: "TestCaseExecutionIdFk");

            migrationBuilder.AddForeignKey(
                name: "FK_TestStepsExecution_TestCaseExecution_TestCaseExecutionId",
                table: "TestStepsExecution",
                column: "TestCaseExecutionId",
                principalTable: "TestCaseExecution",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestStepsExecution_TestCaseExecution_TestCaseExecutionIdFk",
                table: "TestStepsExecution",
                column: "TestCaseExecutionIdFk",
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

            migrationBuilder.DropForeignKey(
                name: "FK_TestStepsExecution_TestCaseExecution_TestCaseExecutionIdFk",
                table: "TestStepsExecution");

            migrationBuilder.DropIndex(
                name: "IX_TestStepsExecution_TestCaseExecutionIdFk",
                table: "TestStepsExecution");

            migrationBuilder.DropColumn(
                name: "TestCaseExecutionIdFk",
                table: "TestStepsExecution");

            migrationBuilder.AlterColumn<int>(
                name: "TestCaseExecutionId",
                table: "TestStepsExecution",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TestCaseExecutionId1",
                table: "TestStepsExecution",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestStepsExecution_TestCaseExecutionId1",
                table: "TestStepsExecution",
                column: "TestCaseExecutionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TestStepsExecution_TestCaseExecution_TestCaseExecutionId",
                table: "TestStepsExecution",
                column: "TestCaseExecutionId",
                principalTable: "TestCaseExecution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestStepsExecution_TestCaseExecution_TestCaseExecutionId1",
                table: "TestStepsExecution",
                column: "TestCaseExecutionId1",
                principalTable: "TestCaseExecution",
                principalColumn: "Id");
        }
    }
}
