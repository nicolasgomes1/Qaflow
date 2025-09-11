using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixIntegrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JiraProjectKey",
                table: "Integrations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProjectsId",
                table: "Integrations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_ProjectsId",
                table: "Integrations",
                column: "ProjectsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Integrations_Projects_ProjectsId",
                table: "Integrations",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Integrations_Projects_ProjectsId",
                table: "Integrations");

            migrationBuilder.DropIndex(
                name: "IX_Integrations_ProjectsId",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "JiraProjectKey",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "ProjectsId",
                table: "Integrations");
        }
    }
}
