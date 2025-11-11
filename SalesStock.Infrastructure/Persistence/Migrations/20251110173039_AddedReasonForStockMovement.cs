using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesStock.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedReasonForStockMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "StockMovements",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "StockMovements");
        }
    }
}
