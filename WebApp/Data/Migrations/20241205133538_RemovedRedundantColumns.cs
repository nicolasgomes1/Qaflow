using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class RemovedRedundantColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedTestCasesIds",
                table: "TestPlans");

            migrationBuilder.DropColumn(
                name: "SelectedTestCaseIds",
                table: "TestExecution");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedTestCasesIds",
                table: "TestPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SelectedTestCaseIds",
                table: "TestExecution",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
