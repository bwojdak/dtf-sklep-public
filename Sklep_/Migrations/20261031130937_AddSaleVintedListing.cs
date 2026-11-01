using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sklep_.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleVintedListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VintedListingId",
                table: "Sales",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_VintedListingId",
                table: "Sales",
                column: "VintedListingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_VintedListings_VintedListingId",
                table: "Sales",
                column: "VintedListingId",
                principalTable: "VintedListings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_VintedListings_VintedListingId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_VintedListingId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "VintedListingId",
                table: "Sales");
        }
    }
}
