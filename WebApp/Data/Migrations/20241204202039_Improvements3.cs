using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class Improvements3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_Projects_TcProjectId",
                table: "TestCases");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCasesFiles_Projects_TcfProjectId",
                table: "TestCasesFiles");

            migrationBuilder.RenameColumn(
                name: "TcfProjectId",
                table: "TestCasesFiles",
                newName: "ProjectsId");

            migrationBuilder.RenameIndex(
                name: "IX_TestCasesFiles_TcfProjectId",
                table: "TestCasesFiles",
                newName: "IX_TestCasesFiles_ProjectsId");

            migrationBuilder.RenameColumn(
                name: "TcProjectId",
                table: "TestCases",
                newName: "ProjectsId");

            migrationBuilder.RenameIndex(
                name: "IX_TestCases_TcProjectId",
                table: "TestCases",
                newName: "IX_TestCases_ProjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCases_Projects_ProjectsId",
                table: "TestCases",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestCasesFiles_Projects_ProjectsId",
                table: "TestCasesFiles",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_Projects_ProjectsId",
                table: "TestCases");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCasesFiles_Projects_ProjectsId",
                table: "TestCasesFiles");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "TestCasesFiles",
                newName: "TcfProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TestCasesFiles_ProjectsId",
                table: "TestCasesFiles",
                newName: "IX_TestCasesFiles_TcfProjectId");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "TestCases",
                newName: "TcProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_TestCases_ProjectsId",
                table: "TestCases",
                newName: "IX_TestCases_TcProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCases_Projects_TcProjectId",
                table: "TestCases",
                column: "TcProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestCasesFiles_Projects_TcfProjectId",
                table: "TestCasesFiles",
                column: "TcfProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
