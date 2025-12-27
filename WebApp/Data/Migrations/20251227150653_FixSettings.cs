using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "QAflowSettings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "QAflowSettings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "QAflowSettings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "QAflowSettings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectsId",
                table: "QAflowSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_QAflowSettings_ProjectsId",
                table: "QAflowSettings",
                column: "ProjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_QAflowSettings_Projects_ProjectsId",
                table: "QAflowSettings",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QAflowSettings_Projects_ProjectsId",
                table: "QAflowSettings");

            migrationBuilder.DropIndex(
                name: "IX_QAflowSettings_ProjectsId",
                table: "QAflowSettings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "QAflowSettings");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "QAflowSettings");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "QAflowSettings");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "QAflowSettings");

            migrationBuilder.DropColumn(
                name: "ProjectsId",
                table: "QAflowSettings");
        }
    }
}
