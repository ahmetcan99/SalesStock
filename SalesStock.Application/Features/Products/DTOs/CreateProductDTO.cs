namespace SalesStock.Application.Features.Products.DTOs
{
    public class CreateProductDTO
    {
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal VatRate { get; set; }
        public decimal UnitPrice { get; set; }
        public string BarCode { get; set; } = string.Empty;
    }
}
