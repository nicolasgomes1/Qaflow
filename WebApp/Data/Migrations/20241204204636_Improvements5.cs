using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class Improvements5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bugs_Projects_BProjectId",
                table: "Bugs");

            migrationBuilder.DropForeignKey(
                name: "FK_BugsFiles_Projects_BfProjectId",
                table: "BugsFiles");

            migrationBuilder.RenameColumn(
                name: "BfProjectId",
                table: "BugsFiles",
                newName: "ProjectsId");

            migrationBuilder.RenameIndex(
                name: "IX_BugsFiles_BfProjectId",
                table: "BugsFiles",
                newName: "IX_BugsFiles_ProjectsId");

            migrationBuilder.RenameColumn(
                name: "BProjectId",
                table: "Bugs",
                newName: "ProjectsId");

            migrationBuilder.RenameIndex(
                name: "IX_Bugs_BProjectId",
                table: "Bugs",
                newName: "IX_Bugs_ProjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bugs_Projects_ProjectsId",
                table: "Bugs",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BugsFiles_Projects_ProjectsId",
                table: "BugsFiles",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bugs_Projects_ProjectsId",
                table: "Bugs");

            migrationBuilder.DropForeignKey(
                name: "FK_BugsFiles_Projects_ProjectsId",
                table: "BugsFiles");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "BugsFiles",
                newName: "BfProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_BugsFiles_ProjectsId",
                table: "BugsFiles",
                newName: "IX_BugsFiles_BfProjectId");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "Bugs",
                newName: "BProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Bugs_ProjectsId",
                table: "Bugs",
                newName: "IX_Bugs_BProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bugs_Projects_BProjectId",
                table: "Bugs",
                column: "BProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BugsFiles_Projects_BfProjectId",
                table: "BugsFiles",
                column: "BfProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
