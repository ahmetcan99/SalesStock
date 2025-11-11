using SalesStock.Domain.Enums;

namespace SalesStock.Application.Features.Stock.DTOs
{
    public class StockMovementDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public StockMovementType MovementType { get; set; }
        public string ReferenceNo { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
