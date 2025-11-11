namespace SalesStock.Application.Features.PriceLists.DTOs
{
    public class PriceListItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; } 
        public DateTime ValidTo { get; set; }
    }
}
