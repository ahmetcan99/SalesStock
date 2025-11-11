using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesStock.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedProductInfoOnOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductSKU",
                table: "PriceListItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductSKU",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductSKU",
                table: "PriceListItems");

            migrationBuilder.DropColumn(
                name: "ProductSKU",
                table: "OrderItems");
        }
    }
}
