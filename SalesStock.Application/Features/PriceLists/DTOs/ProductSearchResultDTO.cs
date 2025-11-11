namespace SalesStock.Application.Features.PriceLists.DTOs
{
    public class ProductSearchResultDTO
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
