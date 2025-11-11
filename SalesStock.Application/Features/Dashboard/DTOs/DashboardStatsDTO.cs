

namespace SalesStock.Application.Features.Dashboard.DTOs
{
    public class DashboardStatsDTO
    {
        public decimal SalesTotal { get; set; }
        public int OrdersCount { get; set; }
        public int LowStockCount { get; set; }
        public IReadOnlyList<DashboardOrderRowDTO> LastOrders { get; set; } = new List<DashboardOrderRowDTO>();
        public IReadOnlyList<DashboardSalesByCurrencyDTO> SalesByCurrency { get; set; } = new List<DashboardSalesByCurrencyDTO>();
    }
}
