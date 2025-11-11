

namespace SalesStock.Application.Features.Reports.DTOs
{
    public class LowStockItemDTO
    {
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int StockOnHand { get; set; }
    }
}
