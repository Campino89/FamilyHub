using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyHub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMealComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealComponents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealMealComponents",
                columns: table => new
                {
                    ComponentsId = table.Column<Guid>(type: "uuid", nullable: false),
                    MealsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealMealComponents", x => new { x.ComponentsId, x.MealsId });
                    table.ForeignKey(
                        name: "FK_MealMealComponents_MealComponents_ComponentsId",
                        column: x => x.ComponentsId,
                        principalTable: "MealComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealMealComponents_Meals_MealsId",
                        column: x => x.MealsId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealComponents_Name",
                table: "MealComponents",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealMealComponents_MealsId",
                table: "MealMealComponents",
                column: "MealsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealMealComponents");

            migrationBuilder.DropTable(
                name: "MealComponents");
        }
    }
}
