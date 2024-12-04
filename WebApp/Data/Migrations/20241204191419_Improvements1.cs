using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class Improvements1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_Projects_RProjectId",
                table: "Requirements");

            migrationBuilder.RenameColumn(
                name: "RProjectId",
                table: "Requirements",
                newName: "ProjectsId");

            migrationBuilder.RenameIndex(
                name: "IX_Requirements_RProjectId",
                table: "Requirements",
                newName: "IX_Requirements_ProjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_Projects_ProjectsId",
                table: "Requirements",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_Projects_ProjectsId",
                table: "Requirements");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "Requirements",
                newName: "RProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Requirements_ProjectsId",
                table: "Requirements",
                newName: "IX_Requirements_RProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_Projects_RProjectId",
                table: "Requirements",
                column: "RProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
