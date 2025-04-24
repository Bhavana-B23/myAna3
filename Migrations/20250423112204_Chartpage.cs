using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataVizNavigator1.Migrations
{
    /// <inheritdoc />
    public partial class Chartpage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChartPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageChartMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageId = table.Column<int>(type: "int", nullable: false),
                    ChartId = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageChartMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageChartMappings_ChartPages_PageId",
                        column: x => x.PageId,
                        principalTable: "ChartPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageChartMappings_Charts_ChartId",
                        column: x => x.ChartId,
                        principalTable: "Charts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageChartMappings_ChartId",
                table: "PageChartMappings",
                column: "ChartId");

            migrationBuilder.CreateIndex(
                name: "IX_PageChartMappings_PageId",
                table: "PageChartMappings",
                column: "PageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageChartMappings");

            migrationBuilder.DropTable(
                name: "ChartPages");
        }
    }
}
