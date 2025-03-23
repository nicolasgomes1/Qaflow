using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedRequirementsSpecification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequirementsSpecificationId",
                table: "Requirements",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RequirementsSpecification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProjectsId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ArchivedStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequirementsSpecification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequirementsSpecification_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_RequirementsSpecificationId",
                table: "Requirements",
                column: "RequirementsSpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementsSpecification_ProjectsId",
                table: "RequirementsSpecification",
                column: "ProjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_RequirementsSpecification_RequirementsSpecific~",
                table: "Requirements",
                column: "RequirementsSpecificationId",
                principalTable: "RequirementsSpecification",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_RequirementsSpecification_RequirementsSpecific~",
                table: "Requirements");

            migrationBuilder.DropTable(
                name: "RequirementsSpecification");

            migrationBuilder.DropIndex(
                name: "IX_Requirements_RequirementsSpecificationId",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "RequirementsSpecificationId",
                table: "Requirements");
        }
    }
}
