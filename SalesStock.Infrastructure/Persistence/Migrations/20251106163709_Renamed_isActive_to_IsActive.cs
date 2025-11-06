using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesStock.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Renamed_isActive_to_IsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "StockMovements",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Products",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "PriceLists",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "PriceListItems",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Orders",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "OrderItems",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Customers",
                newName: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "StockMovements",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Products",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "PriceLists",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "PriceListItems",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Orders",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "OrderItems",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Customers",
                newName: "isActive");
        }
    }
}
