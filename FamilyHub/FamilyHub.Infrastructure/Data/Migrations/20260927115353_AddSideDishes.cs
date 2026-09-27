using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyHub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSideDishes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SideDishes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SideDishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealPlanEntrySideDishes",
                columns: table => new
                {
                    MealPlanEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SideDishesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlanEntrySideDishes", x => new { x.MealPlanEntryId, x.SideDishesId });
                    table.ForeignKey(
                        name: "FK_MealPlanEntrySideDishes_MealPlanEntries_MealPlanEntryId",
                        column: x => x.MealPlanEntryId,
                        principalTable: "MealPlanEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealPlanEntrySideDishes_SideDishes_SideDishesId",
                        column: x => x.SideDishesId,
                        principalTable: "SideDishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanEntrySideDishes_SideDishesId",
                table: "MealPlanEntrySideDishes",
                column: "SideDishesId");

            migrationBuilder.CreateIndex(
                name: "IX_SideDishes_Name",
                table: "SideDishes",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealPlanEntrySideDishes");

            migrationBuilder.DropTable(
                name: "SideDishes");
        }
    }
}
