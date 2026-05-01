using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MechanicalSheets.Api.Migrations
{
    /// <inheritdoc />
    public partial class modifydescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "SheetDefectItems");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DefectCatalogs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "DefectCatalogs");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SheetDefectItems",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
