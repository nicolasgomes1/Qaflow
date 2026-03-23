using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixJira : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JiraIntegrationId",
                table: "Projects",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JiraProjectId",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_JiraIntegrationId",
                table: "Projects",
                column: "JiraIntegrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Integrations_JiraIntegrationId",
                table: "Projects",
                column: "JiraIntegrationId",
                principalTable: "Integrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Integrations_JiraIntegrationId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_JiraIntegrationId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "JiraIntegrationId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "JiraProjectId",
                table: "Projects");
        }
    }
}
