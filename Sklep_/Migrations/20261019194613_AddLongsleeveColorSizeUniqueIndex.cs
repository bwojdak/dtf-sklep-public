using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sklep_.Migrations
{
    /// <inheritdoc />
    public partial class AddLongsleeveColorSizeUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Longsleeves_Color_Size",
                table: "Longsleeves",
                columns: new[] { "Color", "Size" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Longsleeves_Color_Size",
                table: "Longsleeves");
        }
    }
}
