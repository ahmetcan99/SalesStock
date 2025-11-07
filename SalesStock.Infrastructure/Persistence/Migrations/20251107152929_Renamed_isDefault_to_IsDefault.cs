using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesStock.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Renamed_isDefault_to_IsDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isDefault",
                table: "PriceLists",
                newName: "IsDefault");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "PriceLists",
                newName: "isDefault");
        }
    }
}
