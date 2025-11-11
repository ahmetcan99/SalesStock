
namespace SalesStock.Application.Features.Reports.DTOs
{
    public class TopProductDTO
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
        public int QuantitySold { get; set; }
    }
}
