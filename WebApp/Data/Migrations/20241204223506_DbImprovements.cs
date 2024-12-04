using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class DbImprovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestExecution_Projects_TEProjectId",
                table: "TestExecution");

            migrationBuilder.RenameColumn(
                name: "TEProjectId",
                table: "TestExecution",
                newName: "ProjectsId");

            migrationBuilder.RenameIndex(
                name: "IX_TestExecution_TEProjectId",
                table: "TestExecution",
                newName: "IX_TestExecution_ProjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestExecution_Projects_ProjectsId",
                table: "TestExecution",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestExecution_Projects_ProjectsId",
                table: "TestExecution");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "TestExecution",
                newName: "TEProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TestExecution_ProjectsId",
                table: "TestExecution",
                newName: "IX_TestExecution_TEProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestExecution_Projects_TEProjectId",
                table: "TestExecution",
                column: "TEProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
