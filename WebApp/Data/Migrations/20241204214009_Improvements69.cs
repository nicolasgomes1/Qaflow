using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class Improvements69 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequirementsFiles_Requirements_RequirementId",
                table: "RequirementsFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementsFiles_Requirements_RequirementsId",
                table: "RequirementsFiles");

            migrationBuilder.DropIndex(
                name: "IX_RequirementsFiles_RequirementId",
                table: "RequirementsFiles");

            migrationBuilder.DropColumn(
                name: "RequirementId",
                table: "RequirementsFiles");

            migrationBuilder.AlterColumn<int>(
                name: "RequirementsId",
                table: "RequirementsFiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequirementsId1",
                table: "RequirementsFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequirementsFiles_RequirementsId1",
                table: "RequirementsFiles",
                column: "RequirementsId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementsFiles_Requirements_RequirementsId",
                table: "RequirementsFiles",
                column: "RequirementsId",
                principalTable: "Requirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementsFiles_Requirements_RequirementsId1",
                table: "RequirementsFiles",
                column: "RequirementsId1",
                principalTable: "Requirements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequirementsFiles_Requirements_RequirementsId",
                table: "RequirementsFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementsFiles_Requirements_RequirementsId1",
                table: "RequirementsFiles");

            migrationBuilder.DropIndex(
                name: "IX_RequirementsFiles_RequirementsId1",
                table: "RequirementsFiles");

            migrationBuilder.DropColumn(
                name: "RequirementsId1",
                table: "RequirementsFiles");

            migrationBuilder.AlterColumn<int>(
                name: "RequirementsId",
                table: "RequirementsFiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RequirementId",
                table: "RequirementsFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RequirementsFiles_RequirementId",
                table: "RequirementsFiles",
                column: "RequirementId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementsFiles_Requirements_RequirementId",
                table: "RequirementsFiles",
                column: "RequirementId",
                principalTable: "Requirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementsFiles_Requirements_RequirementsId",
                table: "RequirementsFiles",
                column: "RequirementsId",
                principalTable: "Requirements",
                principalColumn: "Id");
        }
    }
}
