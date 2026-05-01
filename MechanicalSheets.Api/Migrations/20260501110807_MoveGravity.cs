using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MechanicalSheets.Api.Migrations
{
    /// <inheritdoc />
    public partial class MoveGravity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gravity",
                table: "SheetDefectItems");

            migrationBuilder.AddColumn<byte>(
                name: "Gravity",
                table: "DefectCatalogs",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gravity",
                table: "DefectCatalogs");

            migrationBuilder.AddColumn<byte>(
                name: "Gravity",
                table: "SheetDefectItems",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
