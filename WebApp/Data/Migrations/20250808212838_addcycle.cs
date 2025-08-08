using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addcycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CycleId",
                table: "TestPlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TestPlansId",
                table: "Cycles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestPlans_CycleId",
                table: "TestPlans",
                column: "CycleId");

            migrationBuilder.CreateIndex(
                name: "IX_Cycles_TestPlansId",
                table: "Cycles",
                column: "TestPlansId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cycles_TestPlans_TestPlansId",
                table: "Cycles",
                column: "TestPlansId",
                principalTable: "TestPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestPlans_Cycles_CycleId",
                table: "TestPlans",
                column: "CycleId",
                principalTable: "Cycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cycles_TestPlans_TestPlansId",
                table: "Cycles");

            migrationBuilder.DropForeignKey(
                name: "FK_TestPlans_Cycles_CycleId",
                table: "TestPlans");

            migrationBuilder.DropIndex(
                name: "IX_TestPlans_CycleId",
                table: "TestPlans");

            migrationBuilder.DropIndex(
                name: "IX_Cycles_TestPlansId",
                table: "Cycles");

            migrationBuilder.DropColumn(
                name: "CycleId",
                table: "TestPlans");

            migrationBuilder.DropColumn(
                name: "TestPlansId",
                table: "Cycles");
        }
    }
}
