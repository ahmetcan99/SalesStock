using SalesStock.Application.Features.Dashboard.DTOs;

namespace SalesStock.Application.Interfaces
{
    public interface IDashBoardRepository
    {
        Task<IReadOnlyList<DashboardSalesByCurrencyDTO>> GetSalesByCurrencyAsync(DateTime startDate, DateTime endDate);
        Task<int> GetOrdersCountAsync(DateTime startDate, DateTime endDate);
        Task<int> GetLowStockCountAsync(int stockThreshold);
        Task<List<DashboardOrderRowDTO>> GetLastOrdersAsync();
    }
}
