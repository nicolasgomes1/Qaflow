using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class Improvements2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequirementsFiles_Projects_RfProjectId",
                table: "RequirementsFiles");

            migrationBuilder.RenameColumn(
                name: "RfProjectId",
                table: "RequirementsFiles",
                newName: "ProjectsId");

            migrationBuilder.RenameIndex(
                name: "IX_RequirementsFiles_RfProjectId",
                table: "RequirementsFiles",
                newName: "IX_RequirementsFiles_ProjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementsFiles_Projects_ProjectsId",
                table: "RequirementsFiles",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequirementsFiles_Projects_ProjectsId",
                table: "RequirementsFiles");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "RequirementsFiles",
                newName: "RfProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_RequirementsFiles_ProjectsId",
                table: "RequirementsFiles",
                newName: "IX_RequirementsFiles_RfProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementsFiles_Projects_RfProjectId",
                table: "RequirementsFiles",
                column: "RfProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
