namespace SalesStock.Application.Features.Products.DTOs
{
    public class ProductDTO
    {
        public string Id { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int StockOnHand { get; set; }
        public bool IsActive { get; set; }
    }
}
