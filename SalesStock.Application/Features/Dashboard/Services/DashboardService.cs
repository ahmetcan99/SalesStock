using SalesStock.Application.Features.Dashboard.DTOs;
using SalesStock.Application.Interfaces;

namespace SalesStock.Application.Features.Dashboard.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashBoardRepository _dashboardRepository;
        public DashboardService(IDashBoardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardStatsDTO> GetAsync(DashboardFilterDTO filter)
        {
            var (start, end) = ResolveRange(filter);

            var salesByCurrency = await _dashboardRepository.GetSalesByCurrencyAsync(start, end);
            var ordersCount = await _dashboardRepository.GetOrdersCountAsync(start, end);
            var lowStock = await _dashboardRepository.GetLowStockCountAsync(filter.LowStockThreshold);
            var lastOrders = await _dashboardRepository.GetLastOrdersAsync();

            var totalSales = salesByCurrency.Sum(x => x.TotalSales);

            return new DashboardStatsDTO
            {
                SalesTotal = totalSales,
                OrdersCount = ordersCount,
                LowStockCount = lowStock,
                LastOrders = lastOrders,
                SalesByCurrency = salesByCurrency
            };
        }

        private static (DateTime start, DateTime end) ResolveRange(DashboardFilterDTO filter)
        {
            var now = DateTime.Now;
            var start = DateTime.Today;
            var end = start.AddDays(1);

            return filter.Range switch
            {
                DashboardQuickRange.Today => (start, end),
                DashboardQuickRange.ThisWeek => (start.AddDays(-(int)DateTime.Today.DayOfWeek + 1), start.AddDays(7)),
                DashboardQuickRange.ThisMonth => (new DateTime(now.Year, now.Month, 1),
                                                  new DateTime(now.Year, now.Month, 1).AddMonths(1)),
                DashboardQuickRange.Custom when filter.StartDate.HasValue && filter.EndDate.HasValue =>
                    (filter.StartDate.Value, filter.EndDate.Value.AddDays(1)),
                _ => (start, end)
            };
        }

    }
}
