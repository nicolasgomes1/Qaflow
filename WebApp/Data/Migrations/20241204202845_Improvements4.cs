using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class Improvements4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestPlans_Projects_TPProjectId",
                table: "TestPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_TestPlansFiles_Projects_TPFProjectId",
                table: "TestPlansFiles");

            migrationBuilder.RenameColumn(
                name: "TPFProjectId",
                table: "TestPlansFiles",
                newName: "ProjectsId");

            migrationBuilder.RenameIndex(
                name: "IX_TestPlansFiles_TPFProjectId",
                table: "TestPlansFiles",
                newName: "IX_TestPlansFiles_ProjectsId");

            migrationBuilder.RenameColumn(
                name: "TPProjectId",
                table: "TestPlans",
                newName: "ProjectsId");

            migrationBuilder.RenameIndex(
                name: "IX_TestPlans_TPProjectId",
                table: "TestPlans",
                newName: "IX_TestPlans_ProjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestPlans_Projects_ProjectsId",
                table: "TestPlans",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestPlansFiles_Projects_ProjectsId",
                table: "TestPlansFiles",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestPlans_Projects_ProjectsId",
                table: "TestPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_TestPlansFiles_Projects_ProjectsId",
                table: "TestPlansFiles");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "TestPlansFiles",
                newName: "TPFProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TestPlansFiles_ProjectsId",
                table: "TestPlansFiles",
                newName: "IX_TestPlansFiles_TPFProjectId");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "TestPlans",
                newName: "TPProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TestPlans_ProjectsId",
                table: "TestPlans",
                newName: "IX_TestPlans_TPProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestPlans_Projects_TPProjectId",
                table: "TestPlans",
                column: "TPProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestPlansFiles_Projects_TPFProjectId",
                table: "TestPlansFiles",
                column: "TPFProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
