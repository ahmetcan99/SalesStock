namespace SalesStock.Application.Features.Stock.DTOs
{
    public class AdjustStockDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
