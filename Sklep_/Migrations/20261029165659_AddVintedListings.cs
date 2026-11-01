using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sklep_.Migrations
{
    /// <inheritdoc />
    public partial class AddVintedListings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VintedListings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VintedItemId = table.Column<long>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Profile = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    ListingState = table.Column<int>(type: "INTEGER", nullable: false),
                    LongsleeveColor = table.Column<int>(type: "INTEGER", nullable: true),
                    LongsleeveSize = table.Column<int>(type: "INTEGER", nullable: true),
                    DesignId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VintedListings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VintedListings_VintedItemId",
                table: "VintedListings",
                column: "VintedItemId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VintedListings");
        }
    }
}
