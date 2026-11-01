using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sklep_.Migrations
{
    /// <inheritdoc />
    public partial class AddSalePrintAndLongsleeveCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LongsleeveCost",
                table: "Sales",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrintCost",
                table: "Sales",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LongsleeveCost",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PrintCost",
                table: "Sales");
        }
    }
}
