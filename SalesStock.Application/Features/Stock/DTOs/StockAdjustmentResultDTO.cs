namespace SalesStock.Application.Features.Stock.DTOs
{
    public class StockAdjustmentResultDTO
    {
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty; 
        public int OldStock { get; set; }
        public int NewStock { get; set; }
        public int AdjustedQuantity { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
