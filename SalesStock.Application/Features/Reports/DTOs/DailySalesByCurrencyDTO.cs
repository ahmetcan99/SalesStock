

namespace SalesStock.Application.Features.Reports.DTOs
{
    public class DailySalesByCurrencyDTO
    {
        public string Currency { get; set; } = "TRY";
        public decimal TotalSales { get; set; }
        public int OrderCount { get; set; }
    }
}
