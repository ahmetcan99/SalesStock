

namespace SalesStock.Application.Features.Dashboard.DTOs
{
    public class DashboardSalesByCurrencyDTO
    {
        public string Currency { get; set; } = "TRY";
        public decimal TotalSales { get; set; }
        public int OrderCount { get; set; }
    }
}
